<template>
    <div class="modal-overlay" @click.self="$emit('close')">
        <div class="modal-container">
            <h2 class="titulo">{{ isEditMode ? 'Editar Ingrediente' : 'Novo Ingrediente' }}</h2>
            <button class="btn-fechar" @click="$emit('close')">X</button>

            <div v-if="message" class="message-box" :class="messageType">
                {{ message }}
            </div>

            <form @submit.prevent="handleSubmit">
                <div class="form-group">
                    <label for="nome">Nome do Ingrediente:</label>
                    <input type="text" id="nome" v-model="ingrediente.nome" placeholder="Ex: Pão de Hambúrguer" required>
                </div>

                <div class="form-group">
                    <label for="unidade">Unidade de Medida:</label>
                    <select id="unidade" v-model="ingrediente.unidadeMedida" required>
                        <option value="Unidade">Unidade</option>
                        <option value="Kg">Kg</option>
                        <option value="Grama">Grama</option>
                        <option value="Litro">Litro</option>
                        <option value="Mililitro">Mililitro</option>
                    </select>
                </div>

                <div class="form-group">
                    <label for="estoqueMinimo">Estoque Mínimo:</label>
                    <input type="number" id="estoqueMinimo" v-model.number="ingrediente.estoqueMinimo" placeholder="Ex: 10" step="0.01" required>
                </div>

                <div class="form-group">
                    <label for="fornecedor">Fornecedor Padrão (Opcional):</label>
                    <input type="text" id="fornecedor" v-model="ingrediente.fornecedorPadrao" placeholder="Ex: Pão do Zé">
                </div>

                <button type="submit" class="btn-enviar" :disabled="loading">
                    {{ loading ? 'Salvando...' : (isEditMode ? 'Salvar Alterações' : 'Criar Ingrediente') }}
                </button>
            </form>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useIngredientesStore } from '@/stores/ingredientes';

const props = defineProps({
  ingredienteParaEditar: {
    type: Object,
    default: null
  }
});

const emit = defineEmits(['close', 'ingrediente-salvo']);

const isEditMode = computed(() => props.ingredienteParaEditar !== null);

const ingredienteStore = useIngredientesStore();

const ingrediente = ref({
    nome: '',
    unidadeMedida: 'Unidade',
    estoqueMinimo: 0,
    fornecedorPadrao: ''
});

const loading = ref(false);
const message = ref('');
const messageType = ref(''); 

onMounted(() => {
    if (isEditMode.value) {
        const data = props.ingredienteParaEditar;
        ingrediente.value = {
            nome: data.nome,
            unidadeMedida: data.unidadeMedida,
            estoqueMinimo: data.estoqueMinimo,
            fornecedorPadrao: data.fornecedorPadrao
        };
    }
});

async function handleSubmit() {
    loading.value = true;
    message.value = '';
    messageType.value = '';

    try {
        if (isEditMode.value) {
            await ingredienteStore.updateIngrediente(props.ingredienteParaEditar.id, ingrediente.value);
            message.value = 'Ingrediente atualizado com sucesso!';
        } else {
            await ingredienteStore.addIngrediente(ingrediente.value);
            message.value = 'Ingrediente criado com sucesso!';
        }
        
        messageType.value = 'success';
        
        setTimeout(() => {
            emit('ingrediente-salvo'); // Avisa estoque.vue para recarregar
            emit('close');
        }, 1500);

    } catch (error) {
        message.value = error.message;
        messageType.value = 'error';
    } finally {
        loading.value = false;
    }
}
</script>

<style scoped>
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

.btn-fechar {
    position: absolute;
    top: 15px;
    right: 15px;
    background: none;
    border: none;
    font-size: 1.2rem;
    cursor: pointer;
    color: #888;
}


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
.form-group textarea,
.form-group select {
    width: 100%;
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 1rem;
    box-sizing: border-box;
    transition: border-color 0.3s, box-shadow 0.3s;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
    border-color: #ff7200;
    box-shadow: 0 0 5px rgba(255, 114, 0, 0.5);
    outline: none;
}

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

.message-box {
    text-align: center;
    padding: 15px;
    margin-bottom: 20px;
    border-radius: 4px;
    font-weight: bold;
}
.message-box.success {
    background-color: #d4edda;
    color: #155724;
}
.message-box.error {
    background-color: #f8d7da;
    color: #721c24;
}
</style>