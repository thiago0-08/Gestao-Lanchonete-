import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

// Verifique se esta URL está correta (sem aspas extras)
const API_URL = 'http://localhost:5138/api/NovoProduto';

export const useProdutosStore = defineStore('produtos', () => {

    // CORREÇÃO 1: Garanta que 'produtos' sempre comece como um array.
    const produtos = ref([]);
    const loading = ref(false);
    const error = ref(null);

    async function fetchProdutos() {
        loading.value = true;
        error.value = null;
        try {
            const response = await axios.get(API_URL);
            const data = response.data; // Pega a resposta da API

            // --- A CORREÇÃO ESTÁ AQUI ---
            // Verificamos se a resposta é um objeto e se tem a propriedade 'products'
            if (data && typeof data === 'object' && Array.isArray(data.products)) {
                // Se sim, pegamos o array de dentro
                produtos.value = data.products;
            } else {
                // Se a API retornar um formato que não esperamos
                console.error('A API /api/Produtos retornou um formato inesperado.', data);
                produtos.value = []; // Mantém como um array vazio para segurança
            }
        } catch (err) {
            error.value = 'Erro ao buscar produtos.';
            produtos.value = []; // Garante que é um array mesmo em caso de erro
        } finally {
            loading.value = false;
        }
    }

    async function addProduto(novoProduto) {
        // Esta função agora é segura, pois 'fetchProdutos' 
        // garantiu que 'produtos.value' é um array.
        try {
            const response = await axios.post(API_URL, novoProduto);
            // Adiciona o novo produto (que veio da API) ao array local
            produtos.value.push(response.data);
            return response.data; // Retorna o produto criado
        } catch (error) {
            console.error("Erro ao adicionar produto:", error);
            throw new Error('Não foi possível adicionar o produto.');
        }
    }

    // Garanta que tudo está sendo retornado
    return {
        produtos,
        loading,
        error,
        fetchProdutos,
        addProduto
    };
});