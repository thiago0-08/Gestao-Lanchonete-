import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

export const useAlertaEstoqueStore = defineStore('alertaEstoque', () => {
  const alertaEstoque = ref(false); 
  const mensagem = ref('');          // Mensagem de alerta
  const quantidade = ref(0);         // Número de itens com estoque baixo
  const itensComAlerta = ref([]);


  async function fetchAlertaEstoque() {
    try {
      const response = await axios.get('http://localhost:5138/api/Alerta/estoque');
      const data = response.data;


      if (Array.isArray(data)) {
        
        if (data.length > 0) {
          alertaEstoque.value = true;
          quantidade.value = data.length;
          itensComAlerta.value = data; 
          mensagem.value = `${data.length} iten(s) com estoque baixo ou zerado.`;
        } 
  
        else {
          alertaEstoque.value = false;
          quantidade.value = 0;
          itensComAlerta.value = []; 
          mensagem.value = "Nenhum alerta de estoque. Tudo em ordem!";
        }
      } 
      else {
        throw new Error("Formato de resposta inesperado da API.");
      }

    } catch (error) {
      console.error("Erro ao buscar alertas de estoque:", error);
      alertaEstoque.value = false; 
      quantidade.value = 0;
      itensComAlerta.value = [];
      mensagem.value = "Erro ao carregar dados de alerta.";
    }
  }


  return { alertaEstoque, mensagem, quantidade, itensComAlerta, fetchAlertaEstoque };
});