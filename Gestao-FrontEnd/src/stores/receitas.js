import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

const API_URL = "http://localhost:5138/api/Receitas";

export const useReceitasStore = defineStore('receitas', () => {
   
    const receitas = ref([]);
    const loading = ref(false); 
    const error = ref(null);   

    async function fetchReceitas() {
        loading.value = true;
        error.value = null;
        try {
            const response = await axios.get(API_URL);
            receitas.value = response.data;
        } catch (err) {
            error.value = 'Erro ao buscar as receitas.';
            console.error(err);
            receitas.value = []; // Garante que é um array mesmo em caso de erro
        } finally {
            loading.value = false;
        }
    }

    async function addReceita(novaReceita) {
        try {
            const response = await axios.post(API_URL, novaReceita);
            receitas.value.push(response.data);
            return response.data;
        } catch (err) {
            console.error("Erro ao adicionar receita:", err);
            throw new Error('Não foi possível criar a receita.');
        }
    }

    
    async function updateReceita(id, receitaAtualizada) {
        try {
            const response = await axios.put(`${API_URL}/${id}`, receitaAtualizada);
            // Atualiza a receita na lista local
            const index = receitas.value.findIndex(r => r.id === id);
            if (index !== -1) {
                receitas.value[index] = response.data;
            }
            return response.data;
        } catch (err) {
            console.error("Erro ao atualizar receita:", err);
            throw new Error('Não foi possível atualizar a receita.');
        }
    }
 
    return { 
        receitas, 
        loading,
        error,
        addReceita, 
        fetchReceitas,
        updateReceita
    };
});