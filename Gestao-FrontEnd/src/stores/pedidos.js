import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

export const usePedidosStore = defineStore('pedidos', () => {
    const pedidos = ref([]);
    const loading = ref(false);
    const error = ref(null);

    // Função para buscar todos os pedidos da API
    async function fetchPedidos() {
        loading.value = true;
        error.value = null;
        try {
            const response = await axios.get('http://localhost:5138/api/Pedidos');
            pedidos.value = response.data;
        } catch (err) {
            error.value = 'Erro ao carregar pedidos: ' + err.message;
            console.error('Erro ao buscar pedidos:', err);
        } finally {
            loading.value = false;
        }
    }

    // Função para adicionar um novo pedido
    async function addPedido(novoPedido) {
        try {
            const response = await axios.post('http://localhost:5138/api/Pedidos', novoPedido);
            // Adiciona o novo pedido à lista local e retorna o pedido criado pela API
            pedidos.value.push(response.data);
            return response.data;
        } catch (err) {
            error.value = 'Erro ao adicionar pedido: ' + err.message;
            console.error('Erro ao adicionar pedido:', err);
            throw err;
        }
    }

    // Função para deletar um pedido
    async function deletePedido(id) {
        try {
            await axios.delete(`http://localhost:5138/api/Pedidos/${id}`);
            // Remove o pedido da lista local
            pedidos.value = pedidos.value.filter(p => p.id !== id);
        } catch (err) {
            error.value = 'Erro ao deletar pedido: ' + err.message;
            console.error('Erro ao deletar pedido:', err);
            throw err;
        }
    }

    // Função para atualizar um pedido
    async function updatePedido(id, pedidoAtualizado) {
        try {
            await axios.put(`http://localhost:5138/api/Pedidos/${id}`, pedidoAtualizado);
            // Atualiza o pedido na lista local
            const index = pedidos.value.findIndex(p => p.id === id);
            if (index !== -1) {
                pedidos.value[index] = { ...pedidos.value[index], ...pedidoAtualizado };
            }
        } catch (err) {
            error.value = 'Erro ao atualizar pedido: ' + err.message;
            console.error('Erro ao atualizar pedido:', err);
            throw err;
        }
    }

    return {
        pedidos,
        loading,
        error,
        fetchPedidos,
        addPedido,
        deletePedido,
        updatePedido
    };
});