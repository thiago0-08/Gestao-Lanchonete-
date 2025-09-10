<template>
  <canvas id="faturamento Mensal"></canvas>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { Chart, registerables } from 'chart.js';

// Registra todos os componentes do Chart.js
Chart.register(...registerables);

// O código de inicialização do gráfico agora está dentro do onMounted
onMounted(() => {
  const ctx = document.getElementById('faturamento Mensal');

  new Chart(ctx, {
    type: 'line',
    data: {
      labels: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
      datasets: [{
        label: 'Produtos vendidos',
        data: [1200, 1100, 1500, 2321, 1900, 800, 2500, 2300, 1800, 2200, 2400, 3000],
        borderWidth: 1
      }]
    },
    options: {
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  });
});
</script>
