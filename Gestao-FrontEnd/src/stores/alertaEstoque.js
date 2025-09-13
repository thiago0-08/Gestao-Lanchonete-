import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

export const aletaEstoque = defineStore('alertaEstoque', () => {
  const alertaEstoque = ref(false); 
  const mensagem = ref(''); 
  const quantidade = ref(0); 

  function setAlertaEstoque(value) {
    alertaEstoque.value = value;
  }

  async function fetchAlertaEstoque() {
    try {
      const response = await axios.get('https://localhost:7298/api/Alerta/estoque');
      
      const apiMessage = response.data.mensagem;


      if (apiMessage.includes("Nenhum alerta")) {
        alertaEstoque.value = false;
        quantidade.value = 0;
        mensagem.value = apiMessage;
      } else {
        
        alertaEstoque.value = true;
       
        quantidade.value = response.data.quantidade || 0; 
        mensagem.value = apiMessage;
      }
    } catch (error) {
      console.error("Erro ao buscar alertas de estoque:", error);
      alertaEstoque.value = false; 
      quantidade.value = 0;
      mensagem.value = "Erro ao carregar dados.";
    }
  }

  return { alertaEstoque, mensagem, quantidade, setAlertaEstoque, fetchAlertaEstoque };
});