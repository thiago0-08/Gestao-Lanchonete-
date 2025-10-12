import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

// URL da sua API de produtos
const API_URL = 'http://localhost:5138/api/Produtos';

export const useProdutosStore = defineStore('produtos', () => {
    // State: Onde os dados ficam armazenados
    const produtos = ref([]);
    const loading = ref(false);
    const error = ref(null);

    // Action: Função para buscar os dados da API
    async function fetchProdutos() {
        loading.value = true;
        error.value = null;
        try {
            // Faz a chamada GET para a API
            const response = await axios.get(API_URL);
            // Armazena a lista de produtos no state
            produtos.value = response.data;
        } catch (err) {
            console.error('Erro ao buscar produtos:', err);
            error.value = 'Não foi possível carregar os produtos.';
        } finally {
            loading.value = false;
        }
    }

    // Retorna o state e as actions para que os componentes possam usar
    return {
        produtos,
        loading,
        error,
        fetchProdutos
    };
});