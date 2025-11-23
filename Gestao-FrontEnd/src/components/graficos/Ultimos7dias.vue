<template>
  <canvas id="ultimos7diasChart"></canvas>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

// 1. Define o 'props' que o componente espera
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
  const ctx = document.getElementById('ultimos7diasChart');
  if (!ctx) return; // Garante que o canvas existe

  // Destrói o gráfico antigo
  if (chartInstance.value) {
    chartInstance.value.destroy();
  }

  // Cria o novo gráfico com os dados do 'props'
  chartInstance.value = new Chart(ctx, {
    type: 'pie', // Gráfico de barras
    data: {
      labels: props.chartData.labels, // Labels dinâmicos
      datasets: [{
        label: 'Vendas (R$)',
        data: props.chartData.data, // Dados dinâmicos
        borderWidth: 1,
        backgroundColor: 'rgba(75, 192, 192, 0.7)',
        borderColor: 'rgba(75, 192, 192, 1)',
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

// 3. Renderiza o gráfico quando o componente é montado
onMounted(() => {
  renderChart();
});

// 4. Observa mudanças nos dados e atualiza o gráfico
watch(() => props.chartData, () => {
  renderChart();
}, { deep: true }); // 'deep' é bom para objetos
</script>