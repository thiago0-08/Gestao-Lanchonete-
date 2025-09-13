import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

export const entradaSaidaEstoque = defineStore('entradaSaidaEstoque', () => {
  const entradas = ref([]); 
  const saidas = ref([]);
  const produtoId = ref(null);
  const quantidade = ref(0);
  const unidadeMedida = ref(''); // Corrigido 'unidadademedida'

  // Função para buscar todas as entradas e saídas da API
  async function fetchEntradaSaida() {
    try {
      const response = await axios.get('https://localhost:7298/api/EntradaSaida');
      // Supondo que sua API retorne um array de objetos
      entradas.value = response.data.filter(item => item.tipo === 'entrada');
      saidas.value = response.data.filter(item => item.tipo === 'saida');
    } catch (error) {
      console.error("Erro ao buscar dados:", error);
    }
  }

  // Função para adicionar uma nova entrada ou saída via requisição POST
  async function addEntradaSaida(payload) {
    try {
      // A API precisa de um objeto com 'produtoId', 'quantidade', 'tipo' e 'unidadeMedida'
      const response = await axios.post('https://localhost:7298/api/EntradaSaida', payload);
      console.log('Sucesso!', response.data);
      // Após adicionar, é bom atualizar os dados
      await fetchEntradaSaida();
      return response.data;
    } catch (error) {
      console.error("Erro ao adicionar dados:", error);
      throw error;
    }
  }

  // O store precisa retornar todos os estados e ações para que fiquem acessíveis
  return {
    entradas,
    saidas,
    produtoId,
    quantidade,
    unidadeMedida,
    fetchEntradaSaida,
    addEntradaSaida
  };
});