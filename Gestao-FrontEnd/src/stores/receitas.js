import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

const API_URL = "http://localhost:5138/api/Receitas"; // Verifique se sua URL está correta

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
    
 
    return { 
        receitas, 
        loading,
        error,
        addReceita, 
        fetchReceitas 
    };
});