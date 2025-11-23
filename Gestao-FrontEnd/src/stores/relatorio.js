import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

const RELATORIO_API_URL = 'http://localhost:5138/api/Relatorio';
const VENDAS_API_URL = 'http://localhost:5138/api/Vendas';

export const useRelatorioStore = defineStore('relatorio', () => {
    // --- STATE ---
    const itensEmFalta = ref([]);
    const produtosMaisVendidos = ref([]);
    const faturamentoDiario = ref(0);
    // 👇 NOVOS STATES PARA OS GRÁFICOS 👇
    const faturamentoMensal = ref([]);
    const faturamento7dias = ref([]);
    
    const loading = ref(false);
    const error = ref(null);

    // --- ACTIONS ---
    async function fetchItensEmFalta() { /* ... (função existente) ... */ }
    async function fetchProdutosMaisVendidos(topN = 5) { /* ... (função existente) ... */ }
    async function fetchFaturamentoDiario(data) { /* ... (função existente) ... */ }

    // 👇 NOVA FUNÇÃO 👇
    async function fetchVendasUltimos7Dias() {
        try {
            const response = await axios.get(`${VENDAS_API_URL}/vendas-ultimos-7-dias`);
            faturamento7dias.value = response.data;
        } catch (err) {
            console.error("Erro ao buscar vendas dos últimos 7 dias:", err);
            faturamento7dias.value = [];
        }
    }

    // 👇 NOVA FUNÇÃO 👇
    async function fetchFaturamentoMensal(ano) {
        try {
            const response = await axios.get(`${VENDAS_API_URL}/faturamento-mensal/${ano}`);
            faturamentoMensal.value = response.data;
        } catch (err) {
            console.error("Erro ao buscar faturamento mensal:", err);
            faturamentoMensal.value = [];
        }
    }

    // Função para carregar todos os dados
    async function fetchRelatorioCompleto() {
        loading.value = true;
        error.value = null;
        try {
            const anoAtual = new Date().getFullYear();
            await Promise.all([
                fetchItensEmFalta(),
                fetchProdutosMaisVendidos(5),
                fetchFaturamentoDiario(new Date()),
                fetchVendasUltimos7Dias(), // 👈 Chamar nova função
                fetchFaturamentoMensal(anoAtual) // 👈 Chamar nova função
            ]);
        } catch (err) {
            error.value = "Erro ao carregar relatórios.";
        } finally {
            loading.value = false;
        }
    }

    return {
        itensEmFalta,
        produtosMaisVendidos,
        faturamentoDiario,
        faturamentoMensal, // 👈 Expor novo state
        faturamento7dias, // 👈 Expor novo state
        loading,
        error,
        fetchRelatorioCompleto
        // Não é necessário expor os fetches individuais se apenas o fetchRelatorioCompleto for usado
    };
});