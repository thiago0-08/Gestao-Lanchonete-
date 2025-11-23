<template>
  <div class="container">

    <h2>Bem-vindo ao Dashboard</h2>
    <p>Aqui você pode ver uma visão geral do sistema de gestão da lanchonete.</p>

    <div v-if="loading" class="loading-state">Carregando dados...</div>

    <div v-else class="infos">
      <div class="info-box">
        <div class="icone green">
          <i class="fa-solid fa-dollar-sign"></i>
        </div>
        <div class="info-content">
          <h3>Faturamento Hoje</h3>
          <p>R$ {{ resumoDia.faturamento.toFixed(2) }}</p>
        </div>
      </div>
      <div class="info-box">
        <div class="icone blue">
          <i class="fa-solid fa-file"></i>
        </div>
        <div class="info-content">
          <h3>Pedidos de Hoje</h3>
          <table><p>{{ resumoDia.totalPedidos }}</p></table>
          
        </div>
      </div>
      <div class="info-box">
        <div class="icone orange">
          <i class="fa-solid fa-ticket"></i>
        </div>
        <div class="info-content">
          <h3>Ticket Médio</h3>
          <p>R$ {{ resumoDia.ticketMedio.toFixed(2) }}</p>
        </div>
      </div>
      <div class="info-box">
        <div class="icone red">
          <i class="fa-solid fa-triangle-exclamation"></i>
        </div>
        <div class="info-content">
          <h3>Itens em Alerta</h3>
          <div class="tooltip-wrapper">
            <p>{{ itensEmFalta.length }}</p>
            <span class="tooltip-text" v-if="itensEmFalta.length > 0">
              {{ itensEmFalta.length }} itens precisam de reposição!
            </span>
          </div>
        </div>
      </div>
    </div>

    <div class="info-vendas" v-if="!loading">
      
      <div class="dashboard-card chart-card">
        <h3>Faturamento Mensal</h3>
        <FaturamentoMensal :chartData="dadosGraficoMensal" />
      </div>

      <div class="dashboard-card table-card">
        <h3>Produtos Mais Vendidos</h3>
        <div class="table-responsive">
          <table>
            <thead>
              <tr>
                <th>Produto</th>
                <th>Qtd. Vendida</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="produto in produtosMaisVendidos" :key="produto.produtoId">
                <td>{{ produto.nomeProduto || produto.NomeProduto }}</td>
                <td>{{ produto.quantidadeVendida || produto.QuantidadeVendida }}</td>
              </tr>
              <tr v-if="produtosMaisVendidos.length === 0">
                <td colspan="2" style="text-align: center;">Sem vendas registradas.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div class="dashboard-card alert-table" v-if="!loading">
      <h3>Alerta de Estoque ({{ itensEmFalta.length }})</h3>
      <div class="table-responsive">
        <table>
          <thead>
            <tr>
              <th>Nome</th>
              <th>Tipo</th>
              <th>Estoque Atual</th>
              <th>Mínimo</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in itensEmFalta" :key="item.nome">
              <td>{{ item.nome }}</td>
              <td>{{ item.tipo }}</td>
              <td>{{ item.estoqueAtual }}</td>
              <td>{{ item.estoqueMinimo }}</td>
            </tr>
            <tr v-if="itensEmFalta.length === 0">
              <td colspan="4" style="text-align: center; color: green;">
                Nenhum alerta de estoque! Tudo ok.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup>
import { onMounted, computed } from 'vue';
import { storeToRefs } from 'pinia';
import Ultimos7dias from '@/components/graficos/Ultimos7dias.vue';
import FaturamentoMensal from '@/components/graficos/FaturamentoMensal.vue';
// Usamos a store unificada de relatórios agora
import { useRelatorioStore } from '../stores/relatorio';

const relatorioStore = useRelatorioStore();

// Extraímos os dados reativos da store
const { 
  resumoDia, 
  itensEmFalta, 
  produtosMaisVendidos, 
  faturamentoMensal, 
  loading 
} = storeToRefs(relatorioStore);

// Computada para formatar os dados para o gráfico de Faturamento Mensal
const dadosGraficoMensal = computed(() => {
  return {
    labels: faturamentoMensal.value.map(m => m.label || m.Label),
    data: faturamentoMensal.value.map(m => m.valor || m.Valor)
  };
});

onMounted(() => {
  // Carrega TODOS os dados necessários para o dashboard de uma vez
  relatorioStore.fetchDadosDashboard();
});
</script>

<style scoped>
.container {
    margin-left: 250px;
    padding: 20px;
    font-family: Arial, sans-serif;
}

h2 {
    color: #333;
    margin-bottom: 20px;
}

.loading-state {
    text-align: center;
    font-size: 1.2rem;
    color: #666;
    margin-top: 50px;
}

/* --- CARDS INFORMATIVOS --- */
.infos {
    margin-top: 30px;
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
    gap: 20px;
}

.info-box {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    display: flex;
    align-items: center;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    transition: transform 0.2s;
}

.info-box:hover {
    transform: translateY(-2px);
}

.icone {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    display: flex;
    justify-content: center;
    align-items: center;
    margin-right: 15px;
    font-size: 20px;
    color: #fff;
    flex-shrink: 0;
}

.icone.green { background-color: #28a745; }
.icone.blue { background-color: #007bff; }
.icone.orange { background-color: #ffc107; }
.icone.red { background-color: #dc3545; }

.info-content {
    display: flex;
    flex-direction: column;
}

.info-box h3 {
    margin: 0 0 5px 0;
    color: #6c757d;
    font-size: 0.9rem;
    font-weight: 600;
}

.info-box p {
    font-size: 1.5rem;
    font-weight: bold;
    color: #333;
    margin: 0;
}

/* --- ÁREA PRINCIPAL (GRÁFICO E TABELA) --- */
.info-vendas {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
    gap: 20px;
    margin-top: 30px;
}

.dashboard-card {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    display: flex;
    flex-direction: column;
}

.chart-card {
    height: 350px; /* Altura fixa para o gráfico */
    min-width: 300px;
}

.table-card {
    height: 350px;
    overflow: hidden; /* Esconde barra de rolagem externa */
}

.table-responsive {
    overflow-y: auto;
    height: 100%;
}

/* --- TABELAS --- */
table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 10px;
}

th, td {
    border-bottom: 1px solid #eee;
    padding: 10px;
    text-align: left;
    font-size: 0.95rem;
}

th {
    background-color: #f8f9fa;
    font-weight: 600;
    color: #555;
    position: sticky;
    top: 0;
}

tbody tr:hover {
    background-color: #f1f1f1;
}

/* Alert Table específica */
.alert-table {
    margin-top: 30px;
}

/* Tooltip simples */
.tooltip-wrapper {
    position: relative;
}
.tooltip-text {
    visibility: hidden;
    width: 180px;
    background-color: #555;
    color: #fff;
    text-align: center;
    border-radius: 6px;
    padding: 5px;
    position: absolute;
    z-index: 1;
    bottom: 125%;
    left: 50%;
    margin-left: -90px;
    opacity: 0;
    transition: opacity 0.3s;
    font-size: 0.8rem;
}
.info-box:hover .tooltip-text {
    visibility: visible;
    opacity: 1;
}

/* Responsividade */
@media (max-width: 768px) {
    .container {
        margin-left: 0;
    }
    .info-vendas {
        grid-template-columns: 1fr;
    }
    .chart-card, .table-card {
        width: 100%;
    }
}
</style>