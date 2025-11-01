<template>
    <div class="modal-overlay" @click.self="$emit('close')">
        <div class="modal-container">
            <h2 class="titulo">{{ isEditMode ? 'Editar Produto' : 'Criar Novo Produto' }}</h2>
            <button class="btn-fechar" @click="$emit('close')">X</button>

            <div v-if="message" :class="['message', messageType]">
                {{ message }}
            </div>

            <form @submit.prevent="salvarProdutoEReceita">
                <div class="form-section">
                    <h4>Dados do Produto</h4>
                    <div class="form-group">
                        <label for="nome">Nome do Produto:</label>
                        <input type="text" id="nome" v-model="produto.nome" required>
                    </div>
                    <div class="form-group">
                        <label for="descricao">Descrição:</label>
                        <textarea id="descricao" v-model="produto.descricao" rows="3"></textarea>
                    </div>
                    <div class="form-group">
                        <label for="preco">Preço (R$):</label>
                        <input type="number" id="preco" v-model.number="produto.preco" step="0.01" required>
                    </div>
                    <div class="form-group">
                        <label for="categoria">Categoria:</label>
                        <select id="categoria" v-model.number="produto.idCategoria" required>
                            <option disabled value="">Selecione uma categoria</option>
                            <option v-for="cat in categorias" :key="cat.id" :value="cat.id">{{ cat.nome }}</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="imagem">URL da Imagem:</label>
                        <input type="text" id="imagem" v-model="produto.imagem">
                    </div>
                </div>

                <button type="button" @click="mostrarReceita = !mostrarReceita" class="btn-toggle-receita">
                    {{ mostrarReceita ? 'Esconder Receita' : 'Adicionar Receita' }}
                </button>

                <div v-if="mostrarReceita" class="form-section slide-down">
                    <h4>Receita do Produto</h4>
                    <div class="form-group">
                        <label for="nomeReceita">Nome da Receita:</label>
                        <input type="text" id="nomeReceita" v-model="receita.nome" placeholder="Ex: Receita do Hambúrguer Especial">
                    </div>
                    
                    <h5>Ingredientes</h5>
                    <div class="item-adder">
                        <select v-model="novoIngrediente.ingredienteId">
                            <option disabled value="">Selecione um ingrediente</option>
                            <option v-for="ing in ingredientes" :key="ing.id" :value="ing.id">{{ ing.nome }}</option>
                        </select>
                        <input type="number" v-model.number="novoIngrediente.quantidade" placeholder="Qtd." step="0.01">
                        <button type="button" @click="adicionarIngrediente" class="btn-add-item">+</button>
                    </div>
                    
                    <ul class="item-list" v-if="receita.itens.length > 0">
                        <li v-for="(item, index) in receita.itens" :key="index">
                            <span>{{ getNomeIngrediente(item.ingredienteId) }} (Qtd: {{ item.quantidade }})</span>
                            <button type="button" @click="removerIngrediente(index)" class="btn-remove-item">×</button>
                        </li>
                    </ul>
                </div>

                <button type="submit" class="btn-enviar" :disabled="loading">
                    {{ loading ? 'Salvando...' : (isEditMode ? 'Salvar Alterações' : 'Salvar Produto e Receita') }}
                </button>
            </form>
            </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useProdutosStore } from '../stores/Produtos';
import { useReceitasStore } from '../stores/receitas';
import { useCategoriasStore } from '../stores/categorias';
import { useIngredientesStore } from '../stores/ingredientes';

// 1. Definir os props que o componente aceita
const props = defineProps({
  produtoParaEditar: {
    type: Object,
    default: null
  },
  receitaParaEditar: {
    type: Object,
    default: null
  }
});

const emit = defineEmits(['close']);

// 2. Definir se estamos em modo de edição
const isEditMode = computed(() => props.produtoParaEditar !== null);

// Instanciando as stores
const produtosStore = useProdutosStore();
const receitasStore = useReceitasStore();
const categoriasStore = useCategoriasStore();
const ingredientesStore = useIngredientesStore();

// State para os formulários
const produto = ref({
  nome: '',
  descricao: '',
  preco: 0,
  idCategoria: '',
  imagem: ''
});

const receita = ref({
  id: null, // Importante para a atualização
  nome: '',
  produtoId: 0,
  itens: []
});

const novoIngrediente = ref({
  ingredienteId: '',
  quantidade: 0
});

// State para controle da UI
const mostrarReceita = ref(false);
const loading = ref(false);
const message = ref('');
const messageType = ref('');

// Listas para preencher os dropdowns
const categorias = ref([]);
const ingredientes = ref([]);

// 3. Atualizar o onMounted
onMounted(async () => {
  // Busca dados dos dropdowns
  await categoriasStore.fetchCategorias();
  categorias.value = categoriasStore.categorias;

  await ingredientesStore.fetchIngredientes();
  ingredientes.value = ingredientesStore.ingredientes;

  // Se estiver em modo de edição, preenche os formulários
  if (isEditMode.value) {
    // Preenche o formulário de produto
    produto.value = {
      nome: props.produtoParaEditar.nome,
      descricao: props.produtoParaEditar.descricao,
      preco: props.produtoParaEditar.preco,
      idCategoria: props.produtoParaEditar.categoria.id, // O backend espera o ID
      imagem: props.produtoParaEditar.imagem
    };

    // Preenche o formulário de receita, se ela existir
    if (props.receitaParaEditar) {
      receita.value = {
        id: props.receitaParaEditar.id,
        nome: props.receitaParaEditar.nome,
        produtoId: props.receitaParaEditar.produtoId,
        // Garante que os itens sejam uma cópia (para evitar reatividade cruzada)
        itens: JSON.parse(JSON.stringify(props.receitaParaEditar.itens || []))
      };
      mostrarReceita.value = true;
    }
  }
});

function getNomeIngrediente(id) {
    const ingrediente = ingredientes.value.find(i => i.id === id);
    return ingrediente ? ingrediente.nome : 'Ingrediente inválido';
}

function adicionarIngrediente() {
  if (!novoIngrediente.value.ingredienteId || novoIngrediente.value.quantidade <= 0) {
    alert('Selecione um ingrediente e informe uma quantidade válida.');
    return;
  }
  // Garante que a quantidade seja um número
  novoIngrediente.value.quantidade = parseFloat(novoIngrediente.value.quantidade) || 0;
  
  receita.value.itens.push({ ...novoIngrediente.value });
  novoIngrediente.value = { ingredienteId: '', quantidade: 0 };
}

function removerIngrediente(index) {
  receita.value.itens.splice(index, 1);
}

// 4. Atualizar a Função de Salvar
async function salvarProdutoEReceita() {
  if (!produto.value.nome || produto.value.preco <= 0 || !produto.value.idCategoria) {
      message.value = 'Preencha os campos obrigatórios do produto.';
      messageType.value = 'error';
      return;
  }

  loading.value = true;
  message.value = '';

  try {
    if (isEditMode.value) {
      // --- LÓGICA DE ATUALIZAÇÃO (EDITAR) ---
      
      // 1. Atualiza o produto
      // O ID vem do prop, não do 'produto.value'
      await produtosStore.updateProduto(props.produtoParaEditar.id, produto.value);
      
      // 2. Atualiza a receita (se ela existir)
      if (mostrarReceita.value && receita.value.id) {
        receita.value.produtoId = props.produtoParaEditar.id; // Garante a consistência
        await receitasStore.updateReceita(receita.value.id, receita.value);
      }
      // Opcional: criar uma receita se ela não existia antes

      message.value = 'Produto atualizado com sucesso!';
      
    } else {
      // --- LÓGICA DE CRIAÇÃO (NOVO) ---
      
      // 1. Criar o produto
      const produtoCriado = await produtosStore.addProduto(produto.value);
      
      // 2. Se a receita estiver visível e tiver itens, criá-la
      if (mostrarReceita.value && receita.value.itens.length > 0) {
        receita.value.produtoId = produtoCriado.id;
        if (!receita.value.nome) {
          receita.value.nome = `Receita para ${produtoCriado.nome}`;
        }
        await receitasStore.addReceita(receita.value);
      }
      message.value = 'Produto e receita salvos com sucesso!';
    }

    messageType.value = 'success';
    setTimeout(() => emit('close'), 2000);

  } catch (error) {
    message.value = `Erro ao salvar: ${error.message}`;
    messageType.value = 'error';
    console.error(error);
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
/* === OVERLAY E CONTAINER === */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal-container {
  background-color: #fff;
  width: 90%;
  max-width: 600px;
  border-radius: 10px;
  padding: 25px 30px;
  position: relative;
  box-shadow: 0 5px 25px rgba(0, 0, 0, 0.2);
  animation: fadeIn 0.3s ease-in-out;
  /* Adicionado para rolagem interna em telas pequenas */
  max-height: 90vh;
  overflow-y: auto;
}

/* === ANIMAÇÕES === */
@keyframes fadeIn {
  from { opacity: 0; transform: scale(0.9); }
  to { opacity: 1; transform: scale(1); }
}

.slide-down {
  animation: slide-down 0.4s ease-out;
}

@keyframes slide-down {
  from {
    opacity: 0;
    transform: translateY(-15px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* === CABEÇALHO === */
.titulo {
  text-align: center;
  margin-bottom: 15px;
  color: #333;
  font-size: 1.5rem;
}

.btn-fechar {
  position: absolute;
  right: 15px;
  top: 15px;
  background: none;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
  color: #888;
  transition: color 0.2s;
}

.btn-fechar:hover {
  color: #e63946;
}

/* === FORMULÁRIO === */
form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-section {
  background-color: #f9fafb;
  border: 1px solid #e0e0e0;
  padding: 15px;
  border-radius: 8px;
}

.form-section h4 {
  margin-bottom: 10px;
  color: #0056b3;
  border-bottom: 1px solid #ccc;
  padding-bottom: 5px;
}

.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 12px;
}

.form-group label {
  font-weight: 600;
  margin-bottom: 5px;
  color: #333;
}

.form-group input,
.form-group textarea,
.form-group select {
  border: 1px solid #ccc;
  border-radius: 5px;
  padding: 8px 10px;
  font-size: 0.95rem;
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  border-color: #007bff;
  outline: none;
}

/* === BOTÕES === */
.btn-toggle-receita,
.btn-enviar {
  border: none;
  border-radius: 6px;
  padding: 10px 12px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s, transform 0.1s;
}

.btn-toggle-receita {
  background-color: #e9ecef;
  color: #333;
}

.btn-toggle-receita:hover {
  background-color: #d6d8db;
}

.btn-enviar {
  background-color: #007bff;
  color: #fff;
}

.btn-enviar:hover {
  background-color: #0069d9;
}

.btn-enviar:disabled {
  background-color: #7da9e8;
  cursor: not-allowed;
}

/* === RECEITA === */
h5 {
  margin: 10px 0 8px;
  color: #444;
}

.item-adder {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 10px;
}

.item-adder select,
.item-adder input {
  flex: 1;
}

.btn-add-item {
  background-color: #28a745;
  color: #fff;
  border: none;
  border-radius: 50%;
  width: 32px;
  height: 32px;
  font-size: 1.2rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-add-item:hover {
  background-color: #218838;
}

.item-list {
  list-style: none;
  padding: 0;
}

.item-list li {
  background-color: #f1f3f5;
  border: 1px solid #ddd;
  padding: 8px 12px;
  border-radius: 6px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
}

.btn-remove-item {
  background: none;
  border: none;
  color: #e63946;
  font-size: 1.2rem;
  cursor: pointer;
}

.btn-remove-item:hover {
  color: #c9184a;
}

/* === MENSAGENS === */
.message {
  text-align: center;
  padding: 10px;
  border-radius: 6px;
  font-weight: 500;
  margin-bottom: 10px;
}

.message.success {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.message.error {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

/* === RESPONSIVIDADE === */
@media (max-width: 500px) {
  .modal-container {
    padding: 20px;
  }

  .item-adder {
    flex-direction: column;
    align-items: stretch;
  }

  .btn-add-item {
    width: 100%;
    border-radius: 6px;
  }
}
</style>