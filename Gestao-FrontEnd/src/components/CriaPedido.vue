<template>
    <div class="modal-overlay" @click.self="$emit('close')">
        <div class="modal-container">
            <h2 class="titulo">Criar Novo Pedido</h2>
            <button class="btn-fechar" @click="$emit('close')">X</button>

            <div v-if="message" :class="['message', messageType]">
                {{ message }}
            </div>

            <form @submit.prevent="submitPedido">
                <div class="form-section">
                    <h4>Dados do Cliente</h4>
                    <div class="form-group">
                        <label for="nomeCliente">Nome do Cliente:</label>
                        <input type="text" id="nomeCliente" v-model="pedido.nomeCliente" placeholder="Nome completo" required>
                    </div>
                    <div class="form-group">
                        <label for="telefoneContato">Telefone:</label>
                        <input type="tel" id="telefoneContato" v-model="pedido.telefoneContato" placeholder="(XX) XXXXX-XXXX" required>
                    </div>
                </div>

                <div class="form-section">
                    <h4>Dados da Entrega</h4>
                    <div class="form-group">
                        <label for="enderecoEntrega">Endereço de Entrega:</label>
                        <input type="text" id="enderecoEntrega" v-model="pedido.enderecoEntrega" placeholder="Rua, Número, Bairro" required>
                    </div>
                    <div class="form-group">
                        <label for="valorEntrega">Valor da Entrega:</label>
                        <input type="number" id="valorEntrega" v-model.number="pedido.valorEntrega" step="0.01" required>
                    </div>
                </div>

                <div class="form-section">
                    <h4>Itens do Pedido</h4>
                    <div class="item-adder">
                        <select v-model="novoItem.produtoId" required>
                            <option disabled value="">Selecione um produto</option>
                            <option v-for="produto in produtos" :key="produto.id" :value="produto.id">
                                {{ produto.nome }}
                            </option>
                        </select>
                        <input type="number" v-model.number="novoItem.quantidade" placeholder="Qtd." min="1" required>
                        <button type="button" @click="adicionarItem" class="btn-add-item">+</button>
                    </div>
                    <ul class="item-list" v-if="pedido.itens.length > 0">
                        <li v-for="(item, index) in pedido.itens" :key="index">
                            <span>{{ getNomeProduto(item.produtoId) }} (Qtd: {{ item.quantidade }})</span>
                            <button type="button" @click="removerItem(index)" class="btn-remove-item">×</button>
                        </li>
                    </ul>
                </div>

                <div class="form-section">
                    <h4>Dados Adicionais</h4>
                    <div class="form-group">
                        <label for="cuponDesconto">Cupom de Desconto (Opcional):</label>
                        <input type="text" id="cuponDesconto" v-model="pedido.cuponDesconto" placeholder="Ex: PROMO10">
                    </div>
                    <div class="form-group">
                        <label for="observacoes">Observações (Opcional):</label>
                        <textarea id="observacoes" v-model="pedido.observacoes" rows="3"></textarea>
                    </div>
                 </div>
                
                <button type="submit" class="btn-enviar">Criar Pedido</button>
            </form>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { usePedidosStore } from '@/stores/pedidos';
// 1. IMPORTAR a nova store de produtos
import { useProdutosStore } from '../stores/Produtos'; 

const emit = defineEmits(['close']);
const pedidosStore = usePedidosStore();
// 2. INSTANCIAR a store de produtos
const produtosStore = useProdutosStore();

// 3. REMOVER A LISTA MOCK e criar uma ref vazia
const produtos = ref([]);

// Objeto reativo para o payload completo do pedido
const pedido = ref({
  itens: [],
  valorEntrega: 0,
  enderecoEntrega: '',
  telefoneContato: '',
  nomeCliente: '',
  cuponDesconto: '',
  observacoes: ''
});

// Objeto para controlar o item que está sendo adicionado
const novoItem = ref({
    produtoId: '',
    quantidade: 1
});

// Mensagens de feedback
const message = ref('');
const messageType = ref('');

// 4. USAR O onMounted PARA BUSCAR OS DADOS QUANDO O COMPONENTE CARREGAR
onMounted(async () => {
  // Chama a action da store para buscar os produtos na API
  await produtosStore.fetchProdutos();
  // Preenche a nossa variável local 'produtos' com os dados da store
  produtos.value = produtosStore.produtos;
});

function getNomeProduto(id) {
    // Agora busca na lista de produtos carregada da API
    const produto = produtos.value.find(p => p.id === id);
    return produto ? produto.nome : 'Produto não encontrado';
}

function adicionarItem() {
    if (!novoItem.value.produtoId || novoItem.value.quantidade <= 0) {
        message.value = 'Selecione um produto e uma quantidade válida.';
        messageType.value = 'error';
        return;
    }
    
    pedido.value.itens.push({ ...novoItem.value });

    novoItem.value.produtoId = '';
    novoItem.value.quantidade = 1;
    message.value = '';
}

function removerItem(index) {
    pedido.value.itens.splice(index, 1);
}

async function submitPedido() {
    if (pedido.value.itens.length === 0) {
        message.value = 'O pedido deve ter pelo menos um item.';
        messageType.value = 'error';
        return;
    }

    try {
        await pedidosStore.addPedido(pedido.value);
        message.value = 'Pedido criado com sucesso!';
        messageType.value = 'success';

        setTimeout(() => {
            emit('close');
        }, 2000);

    } catch (error) {
        message.value = 'Erro ao criar o pedido. Verifique os dados.';
        messageType.value = 'error';
        console.error('Erro ao enviar pedido:', error);
    }
}
</script>

<style scoped>
/* ---------------------------------------------------- */
/* ESTILOS BASE DO MODAL (Você deve ter isso em um CSS global ou aqui, se for o caso) */
/* ---------------------------------------------------- */
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
    background: #ffffff;
    padding: 30px;
    border-radius: 12px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2); 
    width: 90%;
    max-width: 600px; 
    position: relative;
    max-height: 90vh; 
    overflow-y: auto; 
}

/* ---------------------------------------------------- */
/* TÍTULO E BOTÃO DE FECHAR */
/* ---------------------------------------------------- */
.titulo {
    font-size: 1.8rem;
    color: #333; 
    margin-top: 0;
    margin-bottom: 25px;
    border-bottom: 2px solid #007bff; 
    padding-bottom: 10px;
}

.btn-fechar {
    position: absolute;
    top: 15px;
    right: 15px;
    background: none;
    border: none;
    font-size: 1.5rem;
    color: #888;
    cursor: pointer;
    transition: color 0.2s;
    line-height: 1; 
}

.btn-fechar:hover {
    color: #dc3545; 
}

/* ---------------------------------------------------- */
/* SEÇÕES DO FORMULÁRIO */
/* ---------------------------------------------------- */
.form-section {
    margin-bottom: 30px;
    padding: 15px;
    border: 1px solid #e9ecef; 
    border-radius: 8px;
    background-color: #f8f9fa; 
}

.form-section h4 {
    margin-top: 0;
    color: #007bff; 
    border-bottom: 1px solid #dee2e6;
    padding-bottom: 8px;
    margin-bottom: 15px;
}

.form-group {
    margin-bottom: 15px;
}

/* ---------------------------------------------------- */
/* INPUTS E LABELS */
/* ---------------------------------------------------- */
.form-group label {
    display: block; 
    margin-bottom: 5px;
    font-weight: 600; 
    color: #555;
}

input[type="text"],
input[type="tel"],
input[type="number"],
select,
textarea {
    width: 100%; 
    padding: 10px 12px;
    border: 1px solid #ced4da;
    border-radius: 6px;
    font-size: 1rem;
    box-sizing: border-box; 
    transition: border-color 0.2s, box-shadow 0.2s;
}

input:focus,
select:focus,
textarea:focus {
    border-color: #007bff; 
    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25); 
    outline: none;
}

textarea {
    resize: vertical; 
}

/* ---------------------------------------------------- */
/* ADICIONADOR DE ITENS */
/* ---------------------------------------------------- */
.item-adder {
    display: flex;
    gap: 10px;
    align-items: center;
    margin-bottom: 15px;
}

.item-adder select {
    flex-grow: 1;
}

.item-adder input[type="number"] {
    width: 80px; 
    flex-shrink: 0;
    text-align: center;
}

.btn-add-item {
    background-color: #28a745; 
    color: white;
    border: none;
    border-radius: 6px; 
    width: 40px;
    height: 40px;
    font-size: 1.5rem;
    cursor: pointer;
    transition: background-color 0.2s;
    flex-shrink: 0;
}

.btn-add-item:hover {
    background-color: #218838;
}

/* ---------------------------------------------------- */
/* LISTA DE ITENS */
/* ---------------------------------------------------- */
.item-list {
    list-style: none;
    padding: 0;
    margin-top: 10px;
}

.item-list li {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 10px 15px; 
    background-color: #e9ecef; 
    border-left: 4px solid #007bff; 
    border-radius: 6px;
    margin-bottom: 8px; 
    font-size: 0.95rem;
}

.btn-remove-item {
    background: none; 
    color: #dc3545;
    border: none;
    font-size: 1.2rem;
    line-height: 1;
    cursor: pointer;
    transition: color 0.2s;
    padding: 5px;
    margin-left: 10px;
}

.btn-remove-item:hover {
    color: #c82333;
}

/* ---------------------------------------------------- */
/* baotao de envia */
/* ---------------------------------------------------- */
.btn-enviar {
    display: block;
    width: 100%;
    padding: 12px 20px;
    background-color: #007bff; /* Cor primária forte */
    color: white;
    border: none;
    border-radius: 8px;
    font-size: 1.1rem;
    font-weight: bold;
    cursor: pointer;
    transition: background-color 0.2s, transform 0.1s;
    margin-top: 25px;
}

.btn-enviar:hover {
    background-color: #0056b3;
}

.btn-enviar:active {
    transform: translateY(1px);
}


.message {
    padding: 12px;
    margin-bottom: 20px;
    border-radius: 6px;
    text-align: center;
    font-weight: bold;
    border: 1px solid transparent;
}
.success {
    background-color: #d4edda;
    color: #155724;
    border-color: #c3e6cb;
}
.error {
    background-color: #f8d7da;
    color: #721c24;
    border-color: #f5c6cb;
}
</style>