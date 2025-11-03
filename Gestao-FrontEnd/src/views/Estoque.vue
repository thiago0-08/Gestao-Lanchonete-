<template>
  <div class="container">
    <h1>Controle de Estoque</h1>

    <div class="box dashboard-card">
        <button
            class="btn-novo-ingrediente"
            @click="abrirModalCriarIngrediente"
        >
            + Novo Ingrediente
        </button>
        <button
            class="btn-entrada"
            @click="showcriaNovaEntrada = true"
        >
            Registrar Entrada
        </button>
        <button
            class="btn-saida"
            @click="showcriaNovaSaida = true"
        >
            Registrar Saída
        </button>
    </div>

    <div v-if="showcriaNovaEntrada">
        <CriaEntrada 
            @close="showcriaNovaEntrada = false" 
            @lancamento-sucesso="reloadIngredientes"
        />
    </div>

    <div v-if="showcriaNovaSaida">
        <CriaSaida 
            @close="showcriaNovaSaida = false" 
            @lancamento-sucesso="reloadIngredientes"
        />
    </div>

    <div v-if="showCriaNovoIngrediente">
        <CriaIngrediente 
            @close="fecharModalIngrediente" 
            @ingrediente-salvo="reloadIngredientes"
            :ingredienteParaEditar="ingredienteParaEditar"
        />
    </div>

    <div class="box2 dashboard-card">
        <h2>Estoque de Ingredientes</h2>

        <p v-if="loading">Carregando dados de estoque...</p>
        <p v-else-if="ingredientes.length === 0">Nenhum ingrediente cadastrado. Adicione um ingrediente!</p>
      
        <table v-else>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Nome</th>
                    <th>Unidade</th>
                    <th>Estoque Atual</th>
                    <th>Estoque Mínimo</th>
                    <th>Custo Médio</th>
                    <th>Data Validade Próxima</th>
                    <th>Data Última Atualização</th>
                    <th>Status</th>
                    <th>Ações</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in ingredientes" :key="item.id">
                    <td>{{ item.id }}</td>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.unidadeMedida }}</td>
                    <td>{{ item.quantidadeEstoque }}</td>
                    <td>{{ item.estoqueMinimo }}</td>
                    <td>R$ {{ item.custoMedio ? item.custoMedio.toFixed(2) : '0.00' }}</td>
                    <td>{{ formatDate(item.dataValidadeProxima) }}</td>
                    <td>{{ formatDate(item.dataUltimaAtualizacao) }}</td>
                    <td>
                        <span
                            :class="['status-badge', { 'critico': item.quantidadeEstoque < item.estoqueMinimo, 'normal': item.quantidadeEstoque >= item.estoqueMinimo }]">
                            {{ item.quantidadeEstoque < item.estoqueMinimo ? 'Crítico' : 'Normal' }} 
                        </span>
                    </td>
                    <td class="acoes">
                        <button class="btn-editar" @click="abrirModalEditarIngrediente(item)">
                            <font-awesome-icon icon="fa-solid fa-pencil-alt" />
                        </button>
                        <button class="btn-excluir" @click="handleExcluir(item.id)">
                            <font-awesome-icon icon="fa-solid fa-trash-alt" />
                        </button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import CriaSaida from '@/components/EstoqueComponents/CriaSaida.vue';
import CriaEntrada from '@/components/EstoqueComponents/CriaEntrada.vue';
import CriaIngrediente from '@/components/EstoqueComponents/CriaIngrediente.vue';
import { useIngredientesStore } from '@/stores/ingredientes';

const ingredienteStore = useIngredientesStore();

// Usando storeToRefs para manter a reatividade da lista principal
const { ingredientes } = storeToRefs(ingredienteStore);

const loading = ref(true);
const showcriaNovaSaida = ref(false);
const showcriaNovaEntrada = ref(false);

// Refs para o novo modal
const showCriaNovoIngrediente = ref(false);
const ingredienteParaEditar = ref(null);


const reloadIngredientes = async () => {
    loading.value = true;
    try {
        await ingredienteStore.fetchIngredientes();
    } catch (e) {
        console.error("Falha ao recarregar ingredientes:", e);
    } finally {
        loading.value = false;
    }
};


const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    
    return new Date(dateString).toLocaleDateString('pt-BR');
};

onMounted(async () => {
    await reloadIngredientes();
});

// ---  MODAL DE INGREDIENTE ---
function abrirModalCriarIngrediente() {
    ingredienteParaEditar.value = null;
    showCriaNovoIngrediente.value = true;
}

function abrirModalEditarIngrediente(ingrediente) {
    ingredienteParaEditar.value = ingrediente;
    showCriaNovoIngrediente.value = true;
}

function fecharModalIngrediente() {
    ingredienteParaEditar.value = null;
    showCriaNovoIngrediente.value = false;
}

// --- FUNCAO DE EXCLUIR ---
async function handleExcluir(id) {
  if (window.confirm('Tem certeza que deseja excluir este ingrediente? Esta ação não pode ser desfeita.')) {
    try {
      await ingredienteStore.deleteIngrediente(id);
    } catch (err) {
      alert(err.message);
    }
  }
}
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

.box2 {
    padding: 0;
    overflow-x: auto;
}

table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 15px;
    white-space: nowrap; 
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
    position: sticky; 
    top: 0;
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

/* Botão novo ingrediente */
.btn-novo-ingrediente {
    background-color: #007bff;
}
.btn-novo-ingrediente:hover {
    background-color: #0056b3;
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

/* --- Botões na Tabela --- */
.acoes {
    text-align: center;
    white-space: nowrap;
}

.acoes button {
    padding: 6px 10px;
    margin: 0 4px;
    border-radius: 50%;
    font-size: 1rem;
    width: 35px;
    height: 35px;
    line-height: 1.2;
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