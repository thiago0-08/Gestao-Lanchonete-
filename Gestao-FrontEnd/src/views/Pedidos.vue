<template>
  <div class="container">
    <h1>Gestão de Pedidos</h1>

    <div class="info-pedidos dashboard-card">
      <div class="filtros">
        <div class="filter-group">
          <label for="status">Status:</label>
          <select id="status" v-model="statusSelecionado">
            <option value="">Todos</option>
            <option v-for="status in statusOptions" :key="status.value" :value="status.value">
              {{ status.texto }}
            </option>
          </select>
        </div>
        <div class="filter-group">
          <label for="data-filtro">Filtrar por Data:</label>
          <input type="date" id="data-filtro" v-model="dataSelecionada">
        </div>
      </div>
      <button class="btn-novo-pedido" @click="showCriaNovoPedido = true">+ Novo Pedido</button>
    </div>

    <div v-if="showCriaNovoPedido">
      <CriaPedido @close="fecharModalCriacao" />
    </div>

    <div class="tabela-pedidos dashboard-card">
      <div v-if="loading">
        <p>Carregando pedidos...</p>
      </div>
      <div v-else-if="error">
        <p class="error-message">{{ error }}</p>
      </div>
      <div v-else-if="pedidosFiltrados.length === 0">
        <p>Nenhum pedido encontrado para os filtros selecionados.</p>
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
          <tr v-for="pedido in pedidosFiltrados" :key="pedido.id">
            <td>{{ pedido.id }}</td>
            <td>{{ pedido.nomeCliente || 'Não informado' }}</td>
            <td>{{ formatarData(pedido.dataPedido) }}</td>
            <td>
              <span :class="['status-badge', formatarStatusClass(pedido.status)]">
                {{ pedido.status }}
              </span>
            </td>
            <td>R$ {{ pedido.valorTotal?.toFixed(2) ?? '0.00' }}</td>
            <td class="acoes">
              <button class="btn-detalhes" @click="verDetalhes(pedido)">
                <font-awesome-icon icon="fa-solid fa-eye" />
              </button>
              <button class="btn-editar" @click="handleMudarStatus(pedido)">
                <font-awesome-icon icon="fa-solid fa-pencil-alt" />
              </button>
              <button class="btn-excluir" @click="handleExcluir(pedido.id)">
                <font-awesome-icon icon="fa-solid fa-trash-alt" />
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="pedidoSelecionado" class="modal-overlay" @click.self="fecharDetalhes">
        <div class="modal-container">
            <h2 class="titulo">Detalhes do Pedido #{{ pedidoSelecionado.id }}</h2>
            <button class="btn-fechar" @click="fecharDetalhes">X</button>
            
            <div class="detalhes-pedido">
                <p><strong>Cliente:</strong> {{ pedidoSelecionado.nomeCliente || 'Não informado' }}</p>
                <p><strong>Telefone:</strong> {{ pedidoSelecionado.telefoneContato || 'Não informado' }}</p>
                <p><strong>Endereço:</strong> {{ pedidoSelecionado.enderecoEntrega || 'Não informado' }}</p>
                <p><strong>Data:</strong> {{ formatarData(pedidoSelecionado.dataPedido) }}</p>
                <p><strong>Status:</strong> {{ pedidoSelecionado.status }}</p>
                
                <h4 class="titulo-itens">Itens do Pedido:</h4>
                <ul class="lista-itens">
                    <li v-for="item in pedidoSelecionado.itens" :key="item.id">
                        <span>{{ item.produto.nome }}</span>
                        <span>(Qtd: {{ item.quantidade }} | Vl. Unit: R$ {{ item.precoUnitario.toFixed(2) }})</span>
                    </li>
                </ul>
                
                <hr>
                <p><strong>Subtotal:</strong> R$ {{ calcularSubtotal(pedidoSelecionado).toFixed(2) }}</p>
                <p><strong>Taxa de Entrega:</strong> R$ {{ pedidoSelecionado.valorEntrega.toFixed(2) }}</p>
                <p class="total-pedido"><strong>Total:</strong> R$ {{ pedidoSelecionado.valorTotal.toFixed(2) }}</p>
            </div>
        </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { storeToRefs } from 'pinia';
import CriaPedido from '@/components/CriaPedido.vue';
import { usePedidosStore } from '@/stores/pedidos';

const showCriaNovoPedido = ref(false);
const pedidosStore = usePedidosStore();
const { pedidos, loading, error } = storeToRefs(pedidosStore);

const statusSelecionado = ref('');
const dataSelecionada = ref('');
const pedidoSelecionado = ref(null);

const statusOptions = ref([
  { value: 'Pendente', texto: 'Pendente' },
  { value: 'EmPreparacao', texto: 'Em Preparação' },
  { value: 'Entregue', texto: 'Entregue' },
  { value: 'Cancelado', texto: 'Cancelado' }
]);

onMounted(() => {
  pedidosStore.fetchPedidos();
});

const pedidosFiltrados = computed(() => {
  return pedidos.value.filter(pedido => {
    const filtroStatus = !statusSelecionado.value || pedido.status === statusSelecionado.value;
    let filtroData = true;
    if (dataSelecionada.value) {
      const dataPedido = new Date(pedido.dataPedido).setHours(0, 0, 0, 0);
      const [year, month, day] = dataSelecionada.value.split('-').map(Number);
      const dataFiltro = new Date(year, month - 1, day).setHours(0, 0, 0, 0);
      filtroData = (dataPedido === dataFiltro);
    }
    return filtroStatus && filtroData;
  });
});

function formatarData(dateString) {
  if (!dateString) return 'N/A';
  return new Date(dateString).toLocaleDateString('pt-BR', {
    day: '2-digit', month: '2-digit', year: 'numeric'
  });
}

function formatarStatusClass(status) {
 
  if (status === 'EmPreparacao') return 'empreparacao';
  return status.toLowerCase().replace(/ /g, '-');
}

function verDetalhes(pedido) {
  pedidoSelecionado.value = pedido;
}
function fecharDetalhes() {
  pedidoSelecionado.value = null;
}

function fecharModalCriacao() {
  showCriaNovoPedido.value = false;
  pedidosStore.fetchPedidos();
}

async function handleExcluir(id) {
  if (window.confirm('Tem certeza que deseja excluir este pedido?')) {
    try {
      await pedidosStore.deletePedido(id);
    } catch (err) {
      alert('Erro ao excluir o pedido.');
    }
  }
}

// funcao para muda o status do pedido
async function handleMudarStatus(pedido) {
  const listaStatus = statusOptions.value.map(s => s.value).join(', ');
  const novoStatus = window.prompt(`Digite o novo status para o pedido #${pedido.id} (ex: Pendente, EmPreparacao, Entregue, Cancelado):`, pedido.status);


  if (!novoStatus) {
    return; //se o usuario cancelou
  }

  // Validação 
  const statusValido = statusOptions.value.find(s => s.value.toLowerCase() === novoStatus.toLowerCase());
  if (!statusValido) {
    alert(`Status inválido! Por favor, use um dos seguintes: ${listaStatus}`);
    return;
  }

  try {
    // Usamos o 'statusValido.value' para garantir o case correto (ex: 'EmPreparacao')
    await pedidosStore.mudarStatusPedido(pedido.id, statusValido.value);
  } catch (err) {
    alert('Erro ao mudar o status do pedido.');
  }
}

function calcularSubtotal(pedido) {
  return pedido.itens.reduce((total, item) => total + (item.precoUnitario * item.quantidade), 0);
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
    text-transform: capitalize;
}

.status-badge.pendente {
    background-color: #ffc107;
    color: #333;
}

.status-badge.empreparacao { /* Nome da classe vem do formatarStatusClass */
    background-color: #007bff;
}

.status-badge.entregue {
    background-color: #28a745;
}
.status-badge.cancelado {
    background-color: #dc3545;
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
    /* Para os ícones caberem bem */
    align-items: center;
    width: 35px;
    height: 35px;
    line-height: 1.2;
}

.btn-detalhes {
    background-color: #17a2b8;
}
.btn-detalhes:hover {
    background-color: #117a8b;
}

.btn-editar {
    background-color: #117ed1;
}

.btn-editar:hover {
    background-color: #117ed1;
}

.btn-excluir {
    background-color: #dc3545;
}

.btn-excluir:hover {
    background-color: #c82333;
}

/* --- ESTILOS PARA O NOVO MODAL DE DETALHES --- */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.6);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

.modal-container {
    background-color: #fff;
    padding: 30px;
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    width: 90%;
    max-width: 500px;
    position: relative;
    max-height: 90vh;
    overflow-y: auto;
}

.titulo {
    text-align: center;
    color: #333;
    margin-bottom: 20px;
}

.btn-fechar {
    position: absolute;
    top: 15px;
    right: 15px;
    background: none;
    border: none;
    font-size: 1.2rem;
    cursor: pointer;
    color: #888;
}
.btn-fechar:hover {
    color: #333;
}

.detalhes-pedido p {
  margin: 8px 0;
  font-size: 1.1rem;
  color: #333;
}
.detalhes-pedido p strong {
  color: #000;
}
.titulo-itens {
  margin-top: 20px;
  border-top: 1px solid #eee;
  padding-top: 15px;
}
.lista-itens {
  list-style: none;
  padding: 0;
  max-height: 150px;
  overflow-y: auto;
  background-color: #f9f9f9;
  border: 1px solid #eee;
  border-radius: 4px;
}
.lista-itens li {
  padding: 10px;
  display: flex;
  justify-content: space-between;
  border-bottom: 1px solid #eee;
}
.lista-itens li:last-child {
  border-bottom: none;
}
.total-pedido strong {
  font-size: 1.2rem;
  color: #28a745;
}
</style>