<template>
    <div class="container">

        <h2>Bem-vindo ao Dashboard</h2>
        <p>Aqui você pode ver uma visão geral do sistema de gestão da lanchonete.</p>

        <div class="infos">
            <div class="info-box">
                <div class="icone green">
                    <i class="fa-solid fa-dollar-sign"></i>
                </div>
                <div class="info-content">
                    <h3>Faturamento Hoje</h3>
                    <p>R$ 123</p>
                </div>
            </div>
            <div class="info-box">
                <div class="icone blue">
                    <i class="fa-solid fa-file"></i>
                </div>
                <div class="info-content">
                    <h3>Pedidos de Hoje</h3>
                    <p>20</p>
                </div>
            </div>
            <div class="info-box">
                <div class="icone orange">
                    <i class="fa-solid fa-ticket"></i>
                </div>
                <div class="info-content">
                    <h3>Preço Médio</h3>
                    <p>12</p>
                </div>
            </div>
            <div class="info-box">
                <div class="icone red">
                    <i class="fa-solid fa-triangle-exclamation"></i>
                </div>
                <div class="info-content">
                    <h3>Alerta Estoque</h3>
                    <div class="tooltip-wrapper" @mouseenter="showTooltip" @mouseleave="hideTooltip">
                        <p>{{ store.quantidade }}</p>
                        <div v-if="isTooltipVisible" class="tooltip-message">
                            <p>{{ store.mensagem }}</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="info-vendas">
            <div class="dashboard-card chart-card">
                <h3>Vendas Mensais</h3>
                <Ultimos7dias />
            </div>

            <div class="dashboard-card table-card">
                <h3>Produtos Mais Vendidos</h3>
                <table>
                    <thead>
                        <tr>
                            <th>Produto</th>
                            <th>Quantidade Vendida</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Hambúrguer</td>
                            <td>150</td>
                        </tr>
                        <tr>
                            <td>Pizza</td>
                            <td>120</td>
                        </tr>
                        <tr>
                            <td>Refrigerante</td>
                            <td>200</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="dashboard-card  alert-table">
            <h3>Alerta de Estoque</h3>
            <table>
                <thead>
                    <tr>
                        <th>Produto</th>
                        <th>Quantidade em Estoque</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>Pão de Hambúrguer</td>
                        <td>5</td>
                    </tr>
                    <tr>
                        <td>Queijo</td>
                        <td>2</td>
                    </tr>
                    <tr>
                        <td>Tomate</td>
                        <td>4</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import Ultimos7dias from '@/components/graficos/Ultimos7dias.vue';
import { aletaEstoque } from '../stores/alertaEstoque';

const store = aletaEstoque();

onMounted(() => {
    store.fetchAlertaEstoque();
});

const isTooltipVisible = ref(false);

// Função para mostrar o tooltip
const showTooltip = () => {
    isTooltipVisible.value = true;
};

// Função para esconder o tooltip
const hideTooltip = () => {
    isTooltipVisible.value = false;
};
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

.infos {
    margin-top: 60px;
    display: flex;
    gap: 20px;
    flex-wrap: wrap;
}

.info-box {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    flex: 1 1 calc(25% - 20px);
    box-sizing: border-box;
    display: flex;
    align-items: center;
    box-shadow: 0 4px 6px rgb(0, 0, 0);
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
}

.icone.green {
    background-color: #28a745;
}

.icone.blue {
    background-color: #007bff;
}

.icone.orange {
    background-color: #ffc107;
}

.icone.red {
    background-color: #dc3545;
}


.info-content {
    display: flex;
    flex-direction: column;
    text-align: left;
}

.info-box h3 {
    margin-bottom: 5px;
    color: #555;
    font-weight: normal;
    font-size: 1rem;
}

.info-box p {
    font-size: 24px;
    font-weight: bold;
    color: #000;
}

.info-vendas {
    display: flex;
    gap: 20px;
    margin-top: 40px;
}

/* Base para Cards */
.dashboard-card {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 6px rgb(0, 0, 0);
    margin-top: 40px;
}

.chart-card {
    height: 350px;
    width: 650px;
}

.table-card {
    height: 350px;
    width: 650px;
}

/* Tabelas */
table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 15px;
}

th,
td {
    border: 1px solid #e0e0e0;
    padding: 10px;
    text-align: left;
}

th {
    background-color: #f2f2f2;
    font-weight: bold;
    color: #555;
}

tbody tr:nth-child(even) {
    background-color: #f9f9f9;
}

.alert-table th,
.alert-table td {
    border: 1px solid #050505;
}





.tooltip-wrapper {
    position: relative; 
    display: inline-block; 
    cursor: pointer;
}

.tooltip-message {
    position: absolute;
    bottom: calc(100% + 5px); 
    left: 50%; 
    transform: translateX(-50%); 
    background-color: #ff0000;
    color: #fff;
    padding: 8px 12px;
    border-radius: 4px;
    width: 200px; 
    max-width: 250px; 
    text-align: center; 
    z-index: 10;
    font-family: sans-serif;
    font-size: 14px;
}
</style>
