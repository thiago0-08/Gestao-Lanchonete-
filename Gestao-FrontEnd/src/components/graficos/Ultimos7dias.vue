<template>
  <canvas id="myChart"></canvas>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { Chart, registerables } from 'chart.js';

// Registra todos os componentes do Chart.js
Chart.register(...registerables);


onMounted(() => {
  const ctx = document.getElementById('myChart');

  new Chart(ctx, {
    type: 'bar',
    data: {
      labels: ['Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sabado', 'Domingo'],
      datasets: [{
        label: 'Pedidos',
        data: [5, 10, 8, 15, 12, 20, 18],
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
