<template>
  <canvas id="ProdutosMaisVendidos"></canvas>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { Chart, registerables } from 'chart.js';

// Registra todos os componentes do Chart.js
Chart.register(...registerables);

// O código de inicialização do gráfico agora está dentro do onMounted
onMounted(() => {
  const ctx = document.getElementById('ProdutosMaisVendidos');

  new Chart(ctx, {
    type: 'pie',
    data: {
      labels: ['Batata Recheada', 'Coca-Cola', 'X-salada', 'X-Tudo', 'Cachorro-Quente', 'Guarana'],
      datasets: [{
        label: 'Produtos vendidos',
        data: [12, 19, 3, 5, 2, 3],
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
