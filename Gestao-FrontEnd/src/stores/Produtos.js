import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';


const API_URL = 'http://localhost:5138/api/NovoProduto';

export const useProdutosStore = defineStore('produtos', () => {
    
    const produtos = ref([]);
    const loading = ref(false);
    const error = ref(null);

    async function fetchProdutos() {
        loading.value = true;
        error.value = null;
        try {
            const response = await axios.get(API_URL);
            const data = response.data;
            if (data && typeof data === 'object' && Array.isArray(data.products)) {
                produtos.value = data.products;
            } else {
                console.error('A API /api/NovoProduto retornou um formato inesperado.', data);
                produtos.value = [];
            }
        } catch (err) {
            error.value = 'Erro ao buscar produtos.';
            produtos.value = [];
        } finally {
            loading.value = false;
        }
    }

    async function addProduto(novoProduto) {
        try {
            const response = await axios.post(API_URL, novoProduto);
            produtos.value.push(response.data);
            return response.data;
        } catch (error) {
            console.error("Erro ao adicionar produto:", error);
            throw new Error('Não foi possível adicionar o produto.');
        }
    }

    //  FUNÇÃO DE EXCLUIR PRODUTO
    async function deleteProduto(id) {
        try {
            await axios.delete(`${API_URL}/${id}`);
            // Remove o produto da lista local
            produtos.value = produtos.value.filter(p => p.id !== id);
        } catch (err) {
            console.error("Erro ao excluir produto:", err);
            throw new Error('Não foi possível excluir o produto.');
        }
    }

    // FUNÇÃO DE ATUALIZAR PRODUTO
    async function updateProduto(id, produtoAtualizado) {
        try {
            // O DTO do backend espera 'idCategoria', não o objeto Categoria
            const payload = {
                ...produtoAtualizado,
                idCategoria: produtoAtualizado.categoria.id 
            };
            const response = await axios.put(`${API_URL}/${id}`, payload);
            
            // Atualiza o produto na lista local
            const index = produtos.value.findIndex(p => p.id === id);
            if (index !== -1) {
                produtos.value[index] = response.data;
            }
            return response.data;
        } catch (error) {
            console.error("Erro ao atualizar produto:", error);
            throw new Error('Não foi possível atualizar o produto.');
        }
    }

    return {
        produtos,
        loading,
        error,
        fetchProdutos,
        addProduto,
        deleteProduto,  
        updateProduto   
    };
});