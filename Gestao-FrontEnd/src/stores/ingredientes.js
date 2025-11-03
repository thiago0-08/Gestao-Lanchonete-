import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

const API_URL = 'http://localhost:5138/api/Ingrediente';

export const useIngredientesStore = defineStore('ingredientes', () => {
    const ingredientes = ref([]);
    const loading = ref(false);
    const error = ref(null);

    async function fetchIngredientes() {
        loading.value = true;
        error.value = null;
        try {
            const response = await axios.get(API_URL);
            ingredientes.value = response.data;
        } catch (err) {
            console.error("Houve um erro na requisição:", err);
            error.value = "Erro ao buscar ingredientes.";
            ingredientes.value = [];
        } finally {
            loading.value = false;
        }
    }

    //  CRIA um novo ingrediente
    async function addIngrediente(ingredienteDTO) {
        try {
            const response = await axios.post(API_URL, ingredienteDTO);
            ingredientes.value.push(response.data); 
            return response.data;
        } catch (err) {
            console.error("Erro ao adicionar ingrediente:", err);
            throw new Error(err.response?.data?.message || 'Não foi possível criar o ingrediente.');
        }
    }

    //  ATUALIZA um ingrediente existente
    async function updateIngrediente(id, ingredienteDTO) {
        try {
            await axios.put(`${API_URL}/${id}`, ingredienteDTO);

            await fetchIngredientes();
        } catch (err) {
            console.error("Erro ao atualizar ingrediente:", err);
            throw new Error(err.response?.data?.message || 'Não foi possível atualizar o ingrediente.');
        }
    }

    // DELETAR um ingrediente
    async function deleteIngrediente(id) {
        try {
            await axios.delete(`${API_URL}/${id}`);
            ingredientes.value = ingredientes.value.filter(i => i.id !== id);
        } catch (err) {
            console.error("Erro ao deletar ingrediente:", err);
            throw new Error(err.response?.data?.message || 'Não foi possível deletar o ingrediente.');
        }
    }

    return {
        ingredientes,
        loading,
        error,
        fetchIngredientes,
        addIngrediente,    
        updateIngrediente, 
        deleteIngrediente  
    };
});