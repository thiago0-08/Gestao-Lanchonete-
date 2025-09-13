<template>
    <div class="container">
        <h1>Controle de Estoque</h1>
        <div class="box dashboard-card">
            <button class="btn-entrada" @click="showcriaNovaEntrada = true">Registrar Entrada</button>
            <button class="btn-saida" @click="showcriaNovaSaida = true">Registrar Saída</button>
        </div>

        <div v-if="showcriaNovaEntrada">
            <CriaEntrada @close="showcriaNovaEntrada = false" />
        </div>

        <div v-if="showcriaNovaSaida">
            <CriaSaida @close="showcriaNovaSaida = false" />
        </div>

        <div class="box2 dashboard-card">
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nome</th>
                        <th>Unidade de Medida</th>
                        <th>Quantidade em Estoque</th>
                        <th>Estoque Mínimo</th>
                        <th>Custo Médio</th>
                        <th>Fornecedor Padrão</th>
                        <th>Data de Validade Próxima</th>
                        <th>Data da Última Atualização</th>
                        <th>Status</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in estoque" :key="item.id">
                        <td>{{ item.id }}</td>
                        <td>{{ item.nome }}</td>
                        <td>{{ item.unidadeMedida }}</td>
                        <td>{{ item.quantidadeEstoque }}</td>
                        <td>{{ item.estoqueMinimo }}</td>
                        <td>R$ {{ item.custoMedio.toFixed(2) }}</td>
                        <td>{{ item.fornecedorPadrao }}</td>
                        <td>{{ new Date(item.dataValidadeProxima).toLocaleDateString() }}</td>
                        <td>{{ new Date(item.dataUltimaAtualizacao).toLocaleDateString() }}</td>
                        <td>
                            <span
                                :class="['status-badge', { 'critico': item.quantidadeEstoque < item.estoqueMinimo, 'normal': item.quantidadeEstoque >= item.estoqueMinimo }]">
                                {{ item.quantidadeEstoque < item.estoqueMinimo ? 'Crítico' : 'Normal' }} </span>
                        </td>
                        <td class="acoes">
                            <button class="btn-editar"><i class="fas fa-pencil-alt"></i></button>
                            <button class="btn-excluir"><i class="fas fa-trash-alt"></i></button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import CriaSaida from '@/components/EstoqueComponents/CriaSaida.vue';
import CriaEntrada from '@/components/EstoqueComponents/CriaEntrada.vue';
import { Ingrediente } from '@/stores/ingredientes';


const ingredienteStore = Ingrediente();

const ingredientes = ref([]);

//  exibição dos modais
const showcriaNovaSaida = ref(false);
const showcriaNovaEntrada = ref(false);


onMounted(async () => {
    await ingredienteStore.fetchIngredientes();
    ingredientes.value = ingredienteStore.ingredientes;
});

</script>

<style scoped>
.container {
    margin-left: 250px;
    padding: 20px;
    font-family: Arial, sans-serif;
}

h1 {
    color: #333;
    margin-bottom: 20px;
}

/* Base para Cards */
.dashboard-card {
    background: #ffffff;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    margin-top: 20px;
}

.box {
    display: flex;
    gap: 10px;
    margin-bottom: 20px;
}

/* --- Tabela --- */
.box2 {
    padding: 0;
}

table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 15px;
}

th,
td {
    border: 1px solid #e0e0e0;
    padding: 12px;
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

tbody tr:hover {
    background-color: #f1f1f1;
}

/* --- Estilo dos Botões --- */
button {
    padding: 8px 15px;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: all 0.3s ease;
    font-weight: 500;
}

.btn-entrada {
    background-color: #28a745;
}

.btn-entrada:hover {
    background-color: #218838;
}

.btn-saida {
    background-color: #dc3545;
}

.btn-saida:hover {
    background-color: #c82333;
}

/* --- Botões de Ação na Tabela --- */
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

/* --- Status Badges --- */
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

.status-badge.normal {
    background-color: #28a745;
}
</style>