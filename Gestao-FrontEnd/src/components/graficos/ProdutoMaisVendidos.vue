<template>
  <canvas id="ProdutosMaisVendidos"></canvas>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

// 1. Define o 'props' que o componente espera receber
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
  if (!document.getElementById('ProdutosMaisVendidos')) {
    return; // Sai se o canvas não estiver pronto
  }

  const ctx = document.getElementById('ProdutosMaisVendidos');

  // Destrói o gráfico antigo, se ele existir
  if (chartInstance.value) {
    chartInstance.value.destroy();
  }

  // Cria o novo gráfico com os dados do 'props'
  chartInstance.value = new Chart(ctx, {
    type: 'pie', // Gráfico de pizza
    data: {
      labels: props.chartData.labels, // Usa os labels do prop
      datasets: [{
        label: 'Produtos vendidos',
        data: props.chartData.data, // Usa os dados do prop
        borderWidth: 1,
        // Adiciona cores aleatórias para o gráfico de pizza
        backgroundColor: [
          'rgba(255, 99, 132, 0.7)',
          'rgba(54, 162, 235, 0.7)',
          'rgba(255, 206, 86, 0.7)',
          'rgba(75, 192, 192, 0.7)',
          'rgba(153, 102, 255, 0.7)',
        ],
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
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
});
</script>