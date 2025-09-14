<template>
    <div class="modal-overlay" @click.self="$emit('close')">
        <div class="modal-container">
            <h2 class="titulo">Criar Novo Pedido</h2>
            <button class="btn-fechar" @click="$emit('close')">X</button>

            <div v-if="message" :class="['message', messageType]">
                {{ message }}
            </div>

            <form @submit.prevent="submitPedido">
                <div class="form-group">
                    <label for="cliente">Nome do Cliente:</label>
                    <input type="text" id="cliente" v-model="cliente" placeholder="Nome do Cliente" required>
                </div>

                <div class="form-group">
                    <label for="data-pedido">Data do Pedido:</label>
                    <input type="date" id="data-pedido" v-model="dataPedido" required>
                </div>

                <div class="form-group">
                    <label for="total">Valor Total:</label>
                    <input type="number" id="total" v-model.number="total" placeholder="Ex: 50.00" step="0.01" required>
                </div>

                <div class="form-group">
                    <label for="status">Status:</label>
                    <select id="status" v-model="status" required>
                        <option disabled value="">Selecione um status</option>
                        <option value="Pendente">Pendente</option>
                        <option value="Em Andamento">Em Andamento</option>
                        <option value="Concluido">Concluído</option>
                    </select>
                </div>

                <div class="form-group">
                    <label for="observacoes">Observações:</label>
                    <textarea id="observacoes" v-model="observacoes" rows="3" placeholder="Adicione observações adicionais"></textarea>
                </div>
                
                <button type="submit" class="btn-enviar">Enviar</button>
            </form>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue';
import { usePedidosStore } from '@/stores/pedidos';

const emit = defineEmits(['close']);

// Instância da store Pinia
const pedidosStore = usePedidosStore();

// Variáveis reativas para os campos do formulário
const cliente = ref('');
const dataPedido = ref('');
const observacoes = ref('');
const total = ref(0);
const status = ref('');

// Mensagens de feedback para o usuário
const message = ref('');
const messageType = ref('');

// Função para enviar o pedido
async function submitPedido() {
    const payload = {
        cliente: cliente.value,
        data: dataPedido.value,
        total: total.value,
        status: status.value,
        observacoes: observacoes.value
        // Os nomes das propriedades devem coincidir com o DTO da sua API!
    };

    try {
        await pedidosStore.addPedido(payload);
        message.value = 'Pedido criado com sucesso!';
        messageType.value = 'success';

        // Limpa o formulário após o sucesso
        cliente.value = '';
        dataPedido.value = '';
        observacoes.value = '';
        total.value = 0;
        status.value = '';

        setTimeout(() => {
            emit('close');
        }, 3000);
    } catch (error) {
        message.value = 'Erro ao criar o pedido. Por favor, tente novamente.';
        messageType.value = 'error';
        console.error('Erro ao enviar pedido:', error);
    }
}
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
.message {
    text-align: center;
    padding: 15px;
    margin-bottom: 20px;
    border: 1px solid;
    border-radius: 4px;
    font-weight: bold;
}
.message.success {
    background-color: #d4edda;
    color: #155724;
    border-color: #c3e6cb;
}
.message.error {
    background-color: #f8d7da;
    color: #721c24;
    border-color: #f5c6cb;
}
</style>