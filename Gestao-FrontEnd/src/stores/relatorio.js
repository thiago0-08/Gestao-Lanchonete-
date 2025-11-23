import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

const RELATORIO_API_URL = 'http://localhost:5138/api/Relatorio';
const VENDAS_API_URL = 'http://localhost:5138/api/Vendas';

export const useRelatorioStore = defineStore('relatorio', () => {
    // --- STATE ---
    const itensEmFalta = ref([]);
    const produtosMaisVendidos = ref([]);
    
    // Dados do Dashboard
    const resumoDia = ref({
        faturamento: 0,
        totalPedidos: 0,
        ticketMedio: 0
    });

    // Dados dos Gráficos
    const faturamentoMensal = ref([]);
    const faturamento7dias = ref([]);
    
    const loading = ref(false);
    const error = ref(null);

    // --- ACTIONS ---

    // 1. Busca Faturamento, Total de Pedidos e Ticket Médio do dia
    async function fetchResumoDoDia() {
        const hoje = new Date().toISOString().split('T')[0];
        try {
            const [fatRes, pedRes, ticketRes] = await Promise.all([
                axios.get(`${VENDAS_API_URL}/faturamento-diario/${hoje}`),
                axios.get(`${VENDAS_API_URL}/total-pedidos/${hoje}`),
                axios.get(`${VENDAS_API_URL}/ticket-medio/${hoje}`)
            ]);

            resumoDia.value = {
                faturamento: fatRes.data.faturamento,
                totalPedidos: pedRes.data.totalPedidos,
                ticketMedio: ticketRes.data.ticketMedio
            };
        } catch (err) {
            console.error("Erro ao buscar resumo do dia:", err);
        }
    }

    async function fetchItensEmFalta() {
        try {
            const response = await axios.get(`${RELATORIO_API_URL}/itens-em-falta`);
            itensEmFalta.value = response.data;
        } catch (err) {
            console.error("Erro ao buscar itens em falta:", err);
            itensEmFalta.value = [];
        }
    }

    async function fetchProdutosMaisVendidos(topN = 5) {
        try {
            const response = await axios.get(`${VENDAS_API_URL}/mais-vendidos/${topN}`);
            produtosMaisVendidos.value = response.data;
        } catch (err) {
            console.error("Erro ao buscar produtos mais vendidos:", err);
            produtosMaisVendidos.value = [];
        }
    }

    async function fetchVendasUltimos7Dias() {
        try {
            const response = await axios.get(`${VENDAS_API_URL}/vendas-ultimos-7-dias`);
            faturamento7dias.value = response.data;
        } catch (err) {
            console.error("Erro ao buscar vendas 7 dias:", err);
            faturamento7dias.value = [];
        }
    }

    async function fetchFaturamentoMensal(ano) {
        try {
            const response = await axios.get(`${VENDAS_API_URL}/faturamento-mensal/${ano}`);
            faturamentoMensal.value = response.data;
        } catch (err) {
            console.error("Erro ao buscar faturamento mensal:", err);
            faturamentoMensal.value = [];
        }
    }

    // Função Mestra: Carrega TUDO que o Dashboard precisa
    async function fetchDadosDashboard() {
        loading.value = true;
        error.value = null;
        const anoAtual = new Date().getFullYear();
        
        try {
            await Promise.all([
                fetchResumoDoDia(),
                fetchItensEmFalta(),
                fetchProdutosMaisVendidos(5),
                fetchVendasUltimos7Dias(),
                fetchFaturamentoMensal(anoAtual)
            ]);
        } catch (err) {
            error.value = "Erro ao carregar dados do dashboard.";
        } finally {
            loading.value = false;
        }
    }

    return {
        resumoDia,
        itensEmFalta,
        produtosMaisVendidos,
        faturamentoMensal,
        faturamento7dias,
        loading,
        error,
        fetchDadosDashboard
    };
});