<template>
  <canvas id="faturamentoMensalChart"></canvas>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

// 1. Define o 'props'
const props = defineProps({
  chartData: {
    type: Object,
    required: true,
    default: () => ({ labels: [], data: [] })
  }
});

const chartInstance = ref(null);

// 2. Função para criar ou atualizar o gráfico
const renderChart = () => {
  const ctx = document.getElementById('faturamentoMensalChart');
  if (!ctx) return;

  if (chartInstance.value) {
    chartInstance.value.destroy();
  }

  chartInstance.value = new Chart(ctx, {
    type: 'pie', // Gráfico de linha
    data: {
      labels: props.chartData.labels, // Labels dinâmicos (Janeiro, Fevereiro...)
      datasets: [{
        label: 'Faturamento (R$)',
        data: props.chartData.data, // Dados dinâmicos
        borderWidth: 2,
        fill: true,
        backgroundColor: 'rgba(255, 99, 132, 0.2)',
        borderColor: 'rgba(255, 99, 132, 1)',
        tension: 0.1
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  });
};

// 3. Renderiza ao montar
onMounted(() => {
  renderChart();
});

// 4. Observa mudanças nos props
watch(() => props.chartData, () => {
  renderChart();
}, { deep: true });
</script>