import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

export const Ingrediente = defineStore('ingredientes', () => {
    const ingredientes = ref([]);

    async function fetchIngredientes() {
        try {
            const response = await axios.get('http://localhost:5138/api/Ingrediente');
            ingredientes.value = response.data;
        } catch (error) {
            console.error("Houve um erro na requisição:", error);
            ingredientes.value = [];
        }
    }

    return {
        ingredientes,
        fetchIngredientes
    };
});