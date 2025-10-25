import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

const API_URL = "http://localhost:5138/api/Categorias"; 

export const useCategoriasStore = defineStore('categorias', () => {
    const categorias = ref([]);

    async function fetchCategorias() {
        const response = await axios.get(API_URL);
        categorias.value = response.data;
    }
    
    return { categorias, fetchCategorias };
});