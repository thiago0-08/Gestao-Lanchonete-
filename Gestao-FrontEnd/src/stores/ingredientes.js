import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

export const Ingrediente = defineStore('ingredientes', () => {
    const ingredientes = ref([]);

    async function fetchIngredientes() {
        try {
            const response = await axios.get('https://localhost:7298/api/Ingrediente');

            // Verifica se a requisição foi bem-sucedida (status 200-299)
            if (response.status >= 200 && response.status < 300) {
                ingredientes.value = response.data;
            } else {
                throw new Error('Erro ao buscar dados da API. Status: ' + response.status);
            }
        } catch (error) {
            console.error("Houve um erro na requisição:", error);
            // Limpa a lista em caso de erro
            ingredientes.value = [];
        }
    }
    
    // Retorna a lista de ingredientes
    return {
        ingredientes,
        fetchIngredientes
    };
});