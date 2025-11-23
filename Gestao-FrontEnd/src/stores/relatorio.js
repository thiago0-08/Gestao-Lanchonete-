import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';

const RELATORIO_API_URL = 'http://localhost:5138/api/Relatorio';
const VENDAS_API_URL = 'http://localhost:5138/api/Vendas';

export const useRelatorioStore = defineStore('relatorio', () => {
    const itensEmFalta = ref([]);
    const produtosMaisVendidos = ref([]);
    
    // dados do Dashboard
    const resumoDia = ref({
        faturamento: 0,
        totalPedidos: 0,
        ticketMedio: 0
    });

    // Dados dos Gráficos
    const faturamentoMensal = ref([]);
   
    const loading = ref(false);
    const error = ref(null);

   

   
    async function fetchResumoDoDia() {
        const hoje = new Date().toISOString().split('T')[0]; // Data YYYY-MM-DD
        try {
            const [fatRes, pedRes, ticketRes] = await Promise.all([
                axios.get(`${VENDAS_API_URL}/faturamento-diario/${hoje}`),
                axios.get(`${VENDAS_API_URL}/total-pedidos/${hoje}`),
                axios.get(`${VENDAS_API_URL}/ticket-medio/${hoje}`)
            ]);

            resumoDia.value = {
                faturamento: fatRes.data.faturamento || 0,
                totalPedidos: pedRes.data.totalPedidos || 0,
                ticketMedio: ticketRes.data.ticketMedio || 0
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

    

    async function fetchFaturamentoMensal(ano) {
        try {
            const response = await axios.get(`${VENDAS_API_URL}/faturamento-mensal/${ano}`);
            faturamentoMensal.value = response.data;
        } catch (err) {
            console.error("Erro ao buscar faturamento mensal:", err);
            faturamentoMensal.value = [];
        }
    }

   
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
        loading,
        error,
        fetchDadosDashboard
    };
});