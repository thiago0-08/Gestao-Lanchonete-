<template>
    <div class="modal-overlay" @click.self="$emit('close')">
        <div class="modal-container">
            <h2 class="titulo">Saída de Produto</h2>
            <button class="btn-fechar" @click="$emit('close')">X</button>

            <div v-if="message" class="success-message">
                {{ message }}
            </div>

            <form @submit.prevent="submitTicket">
                <div class="form-group">
                    <label for="produto">Nome do Produto:</label>
                    <input type="text" id="produto" v-model="produtoNome" @input="filterProdutos"
                        placeholder="Ex: Hambúrguer" required>

                    <ul v-if="filteredProdutos.length > 0 && produtoNome.length > 0" class="suggestions">
                        <li v-for="item in filteredProdutos" :key="item.id" @click="selectProduto(item)">
                            {{ item.nome }}
                        </li>
                    </ul>
                </div>

                <div class="form-group">
                    <label for="quantidade-saida">Quantidade de saída ({{ selectedProduto?.unidadeMedida || 'Unidade'
                    }})</label>
                    <input type="number" id="quantidade-saida" v-model.number="quantidade" placeholder="Ex: 5"
                        step="0.01" required>
                </div>

                <button type="submit" class="btn-enviar">Enviar</button>
            </form>
        </div>
    </div>
</template>

<script setup>

import { ref, onMounted } from 'vue';
import { useSaidaStore } from '../../stores/SaidaProduto.js';
import { useIngredientesStore } from '@/stores/ingredientes';

const saidaStore = useSaidaStore(); // <-- CORRIGIDO
const ingredienteStore = useIngredientesStore();

const produtoNome = ref('');
const quantidade = ref(0);
const message = ref('');

const ingredientes = ref([]);
const filteredProdutos = ref([]);
const selectedProduto = ref(null);

onMounted(async () => {
    await ingredienteStore.fetchIngredientes();
    ingredientes.value = ingredienteStore.ingredientes;
});

const filterProdutos = () => {
    if (produtoNome.value.length > 0) {
        filteredProdutos.value = ingredientes.value.filter(ing =>
            ing.nome.toLowerCase().includes(produtoNome.value.toLowerCase())
        );
    } else {
        filteredProdutos.value = [];
    }

    selectedProduto.value = null;
};

const selectProduto = (item) => {
    produtoNome.value = item.nome;
    selectedProduto.value = item;
    filteredProdutos.value = [];
};

const submitTicket = async () => {

    if (!selectedProduto.value) {
        message.value = 'Por favor, selecione um produto Válido.';
        return;
    }
    // Validação que o backend exige (> 0)
    if (quantidade.value <= 0) {
        message.value = 'A quantidade de saída deve ser maior que zero.';
        return;
    }

    const payload = {
        idIngrediente: selectedProduto.value.id, // camelCase
        quantidade: quantidade.value,
        unidadeMedida: selectedProduto.value.unidadeMedida,
        tipo: 'saida'
    };

    try {
        await saidaStore.addSaida(payload); // <-- FUNÇÃO CORRIGIDA
        message.value = 'Saída do Produto feita com sucesso!';
        emit('lancamento-sucesso');

        setTimeout(() => {
            produtoNome.value = '';
            quantidade.value = 0;
            selectedProduto.value = null;
            emit('close');
        }, 1500);

    } catch (error) {
        const apiError = error.message.includes('Falha na requisição:')
            ? error.message.replace('Error: Falha na requisição: ', '')
            : 'Erro ao fazer a saída do produto. Verifique o console.';
        message.value = apiError;
        console.error('Erro:', error);
    }
};

const emit = defineEmits(['close', 'lancamento-sucesso']);


</script>

<style scoped>
/* Fundo escuro do modal */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

/* Container principal do modal */
.modal-container {
    background-color: #fff;
    padding: 30px;
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    width: 90%;
    max-width: 500px;
    position: relative;
}

.titulo {
    text-align: center;
    color: #333;
    margin-bottom: 20px;
}

/* Botão de fechar (X) */
.btn-fechar {
    position: absolute;
    top: 15px;
    right: 15px;
    background: none;
    border: none;
    font-size: 1.2rem;
    cursor: pointer;
    color: #888;
    transition: color 0.3s;
}

.btn-fechar:hover {
    color: #333;
}

/* Grupos de formulário */
.form-group {
    margin-bottom: 15px;
}

.form-group label {
    display: block;
    margin-bottom: 5px;
    font-weight: bold;
    color: #555;
}

.form-group input,
.form-group textarea {
    width: 100%;
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 1rem;
    box-sizing: border-box;
    transition: border-color 0.3s, box-shadow 0.3s;
}

.form-group input:focus,
.form-group textarea:focus {
    border-color: #ff7200;
    box-shadow: 0 0 5px rgba(255, 114, 0, 0.5);
    outline: none;
}

/* Botão Enviar */
.btn-enviar {
    display: block;
    width: 100%;
    padding: 12px;
    background-color: #ff7200;
    color: white;
    border: none;
    border-radius: 4px;
    font-size: 1.1rem;
    cursor: pointer;
    margin-top: 15px;
    transition: background-color 0.3s;
}

.btn-enviar:hover {
    background-color: #e65c00;
}

/* Estilo da mensagem de sucesso */
.success-message {
    text-align: center;
    padding: 15px;
    margin-bottom: 20px;
    background-color: #d4edda;
    color: #e65c00;
    border: 1px solid #c3e6cb;
    border-radius: 4px;
    font-weight: bold;
}



/* Adicione estilos para a lista de sugestões */
.suggestions {
    list-style-type: none;
    padding: 0;
    margin-top: 5px;
    border: 1px solid #ccc;
    border-radius: 4px;
    max-height: 150px;
    overflow-y: auto;
    background-color: #fff;
    z-index: 1001;
}

.suggestions li {
    padding: 10px;
    cursor: pointer;
    border-bottom: 1px solid #eee;
}

.suggestions li:hover {
    background-color: #f0f0f0;
}
</style>
