<template>
    <div class="container">
        <h1>Gestão de Pedidos</h1>

        <div class="info-pedidos dashboard-card">
            <div class="filtros">
                <div class="filter-group">
                    <label for="status">Status:</label>
                    <select id="status">
                        <option value="">Todos</option>
                        <option value="pendente">Pendente</option>
                        <option value="em andamento">Em Andamento</option>
                        <option value="concluido">Concluído</option>
                    </select>
                </div>
                <div class="filter-group">
                    <label for="data-inicial">Data Inicial:</label>
                    <input type="date" id="data-inicial">
                </div>
                <button class="btn-filtrar">Filtrar</button>
            </div>
            <button class="btn-novo-pedido" @click="showCriaNovoPedido = true">+ Novo Pedido</button>
        </div>

        <div v-if="showCriaNovoPedido">
            <CriaPedido @close="showCriaNovoPedido = false" />
        </div>

        <div class="tabela-pedidos dashboard-card">
            <div v-if="pedidosStore.loading">
                <p>Carregando pedidos...</p>
            </div>
            <div v-else-if="pedidosStore.error">
                <p class="error-message">{{ pedidosStore.error }}</p>
            </div>
            <table v-else>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Cliente</th>
                        <th>Data</th>
                        <th>Status</th>
                        <th>Total</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="pedido in pedidosStore.pedidos" :key="pedido.id">
                        <td>{{ pedido.id }}</td>
                        <td>{{ pedido.cliente }}</td>
                        <td>{{ new Date(pedido.data).toLocaleDateString() }}</td>
                        <td>
                            <span :class="['status-badge', pedido.status.toLowerCase().replace(/ /g, '-')]">
                                {{ pedido.status }}
                            </span>
                        </td>
                        <td>R$ {{ pedido.valorTotal?.toFixed(2) ?? '0.00' }}</td>
                        <td class="acoes">
                            <button class="btn-editar"><i class="fas fa-pencil-alt"></i></button>
                            <button class="btn-excluir" @click="pedidosStore.deletePedido(pedido.id)"><i
                                    class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import CriaPedido from '@/components/CriaPedido.vue';
import { usePedidosStore } from '@/stores/pedidos';

const showCriaNovoPedido = ref(false);
const pedidosStore = usePedidosStore();

// Chamada à API para buscar os pedidos ao montar o componente
onMounted(() => {
    pedidosStore.fetchPedidos();
});
</script>

<style scoped>
/* Seu CSS não precisa de alterações */
.container {
    margin-left: 250px;
    padding: 20px;
    font-family: Arial, sans-serif;
}

h1 {
    color: #333;
    margin-bottom: 20px;
}

.dashboard-card {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    margin-top: 20px;
}

.info-pedidos {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.filtros {
    display: flex;
    align-items: center;
    gap: 15px;
}

.filter-group {
    display: flex;
    align-items: center;
    gap: 5px;
}

label {
    font-weight: bold;
    color: #555;
}

select,
input[type="date"] {
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    transition: all 0.3s ease;
}

select:focus,
input[type="date"]:focus {
    border-color: #ff7200;
    box-shadow: 0 0 5px rgba(255, 114, 0, 0.5);
    outline: none;
}

button {
    padding: 8px 15px;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: all 0.3s ease;
    font-weight: 500;
}

.btn-filtrar {
    background-color: #007bff;
}

.btn-filtrar:hover {
    background-color: #0056b3;
}

.btn-novo-pedido {
    background-color: #ff7200;
}

.btn-novo-pedido:hover {
    background-color: #e65c00;
}

/* Tabela de pedidos */
.tabela-pedidos table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 15px;
}

.tabela-pedidos th,
.tabela-pedidos td {
    border: 1px solid #e0e0e0;
    padding: 12px;
    text-align: left;
}

.tabela-pedidos th {
    background-color: #f2f2f2;
    font-weight: bold;
    color: #555;
}

.tabela-pedidos tbody tr:nth-child(even) {
    background-color: #f9f9f9;
}

.tabela-pedidos tbody tr:hover {
    background-color: #f1f1f1;
}

/* Status Badges */
.status-badge {
    padding: 4px 8px;
    border-radius: 12px;
    color: #fff;
    font-size: 0.85em;
    font-weight: bold;
}

.status-badge.pendente {
    background-color: #ffc107;
}

.status-badge.em-andamento {
    background-color: #007bff;
}

.status-badge.concluido {
    background-color: #28a745;
}

/* Botões de Ação na Tabela */
.acoes {
    text-align: center;
    white-space: nowrap;
}

.acoes button {
    padding: 6px 10px;
    margin: 0 4px;
    border-radius: 50%;
    font-size: 1rem;
}

.btn-editar {
    background-color: #6c757d;
}

.btn-editar:hover {
    background-color: #5a6268;
}

.btn-excluir {
    background-color: #dc3545;
}

.btn-excluir:hover {
    background-color: #c82333;
}
</style>