<template>
    <div class="container">
        <h1>Relatórios</h1>
        <p class="subtitle">Análises e relatórios do seu negócio</p>

        <div class="filter-card dashboard-card">
        </div>

        <div class="summary-cards-grid">
            <div class="summary-card">
                <p class="summary-label">Faturamento Hoje</p>
                <p class="summary-value green">R$ {{ faturamentoDiario.toFixed(2) }}</p>
            </div>
            <div class="summary-card">
                <p class="summary-label">Itens em Alerta</p>
                <p class="summary-value purple">{{ itensEmFalta.length }}</p>
            </div>
        </div>

        <div class="dashboard-grid">
            <div class="dashboard-card chart-container">
                <h3>Faturamento Mensal</h3>
                <FaturamentoMensal :chartData="dadosGraficoMensal" />
            </div>

            <div class="dashboard-card chart-container">
                <h3>Produtos Mais Vendidos</h3>
                <ProdutoMaisVendidos :chartData="dadosGraficoMaisVendidos" />
            </div>
        </div>

        <div class="dashboard-grid">
            <div class="dashboard-card table-container">
                <h3>Itens em Falta</h3>
                <div v-if="loading" class="loading-message">Carregando itens...</div>
                <div v-else-if="itensEmFalta.length === 0">Nenhum item em falta.</div>
                <table v-else class="data-table">
                </table>
            </div>

            <div class="dashboard-card table-container chart-container">
                <h3>Vendas Últimos 7 dias</h3>
                <Ultimos7dias :chartData="dadosGrafico7Dias" />
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { storeToRefs } from 'pinia';
import FaturamentoMensal from '@/components/graficos/FaturamentoMensal.vue';
import ProdutoMaisVendidos from '@/components/graficos/ProdutoMaisVendidos.vue';
import Ultimos7dias from '@/components/graficos/Ultimos7dias.vue';
import { useRelatorioStore } from '@/stores/relatorio';

const relatorioStore = useRelatorioStore();

// Pega todos os states reativos da store
const {
    itensEmFalta,
    produtosMaisVendidos,
    faturamentoDiario,
    faturamentoMensal,
    faturamento7dias,
    loading,
    error
} = storeToRefs(relatorioStore);

onMounted(() => {
    relatorioStore.fetchRelatorioCompleto();
});

// --- COMPUTED PROPERTIES PARA OS GRÁFICOS ---

// Formata dados para o gráfico de Produtos Mais Vendidos
const dadosGraficoMaisVendidos = computed(() => ({
    labels: produtosMaisVendidos.value.map(p => p.nomeProduto),
    data: produtosMaisVendidos.value.map(p => p.quantidadeVendida)
}));

// 👇 NOVO: Formata dados para o gráfico Faturamento Mensal 👇
const dadosGraficoMensal = computed(() => ({
    labels: faturamentoMensal.value.map(m => m.label),
    data: faturamentoMensal.value.map(m => m.valor)
}));

// 👇 NOVO: Formata dados para o gráfico Últimos 7 Dias 👇
const dadosGrafico7Dias = computed(() => ({
    labels: faturamento7dias.value.map(d => d.label),
    data: faturamento7dias.value.map(d => d.valor)
}));
</script>


<style scoped>
.container {
    margin-left: 250px;
    padding: 20px;
    font-family: Arial, sans-serif;
    background-color: #f7f7f7;
    min-height: 100vh;
}

h1 {
    color: #333;
    margin-bottom: 5px;
}

.subtitle {
    color: #777;
    margin-bottom: 20px;
}

.dashboard-card {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

/* Filtros */
.filter-card {
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
    gap: 15px;
    margin-bottom: 20px;
}

.filter-group {
    display: flex;
    flex-direction: column;
    gap: 5px;
}

.filter-card label {
    font-weight: bold;
    color: #555;
    font-size: 0.9em;
}

.filter-card select,
.filter-card input {
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    transition: all 0.3s ease;
}

.btn-primary {
    padding: 10px 20px;
    background-color: #ff7200;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
    transition: background-color 0.3s ease;
}

.btn-primary:hover {
    background-color: #e65c00;
}

/* Resumo de Vendas */
.summary-cards-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 20px;
    margin-bottom: 20px;
}

.summary-card {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    text-align: center;
}

.summary-label {
    font-weight: bold;
    color: #555;
    margin-bottom: 5px;
}

.summary-value {
    font-size: 2.2em;
    font-weight: bold;
}

.summary-value.green {
    color: #28a745;
}

.summary-value.blue {
    color: #007bff;
}

.summary-value.orange {
    color: #ff7200;
}

.summary-value.purple {
    color: #6f42c1;
}

/* Gráficos e Tabelas */
.dashboard-grid {
    display: grid;
    /* Ajustado para centralizar os cards de 400px */
    grid-template-columns: repeat(auto-fit, 400px);
    gap: 20px;
    margin-top: 20px;
    justify-content: center;
    /* Centraliza os cards se não preencherem a linha */
}

/* --- ESTA É A CORREÇÃO --- */
.chart-container {
    display: flex;
    flex-direction: column;
    width: 400px;
    /* Largura fixa */
    height: 250px;
    /* Altura fixa */
}

/* ------------------------- */

.table-container {
    display: flex;
    flex-direction: column;
}

.data-table {
    width: 100%;
    border-collapse: collapse;
}

.data-table th,
.data-table td {
    border: 1px solid #e0e0e0;
    padding: 12px;
    text-align: left;
}

.data-table th {
    background-color: #f2f2f2;
    font-weight: bold;
    color: #555;
}

.data-table tbody tr:nth-child(even) {
    background-color: #f9f9f9;
}

.data-table tbody tr:hover {
    background-color: #f1f1f1;
}

.status-badge {
    padding: 4px 8px;
    border-radius: 12px;
    color: #fff;
    font-size: 0.85em;
    font-weight: bold;
}

.status-badge.critico {
    background-color: #dc3545;
}

.status-badge.atencao {
    background-color: #ffc107;
}

@media (max-width: 768px) {
    .container {
        margin-left: 0;
    }
}
</style>