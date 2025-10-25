<template>
  <div class="container">
    <h1>Gestão de Receitas</h1>

    <div class="info-receitas dashboard-card">
      <div class="filtros">
        <div class="filter-group">
          <label for="categoria-filtro">Categoria:</label>
          <select id="categoria-filtro" v-model="categoriaSelecionada">
            <option value="">Todas</option>
            <option v-for="categoria in categorias" :key="categoria.id" :value="categoria.id">
              {{ categoria.nome }}
            </option>
          </select>
        </div>
      </div>
      <button class="btn-novo-pedido" @click="showcriaNovoReceita = true"> + Novo Produto </button>
    </div>

    <div v-if="showcriaNovoReceita">
      <CriaProduto @close="showcriaNovoReceita = false" />
    </div>

    <div v-if="loading" class="loading-message">Carregando produtos...</div>
    <div v-else-if="error" class="error-message">{{ error }}</div>
    
    <div v-else class="card-receitas">
      <div 
        v-for="produto in produtosFiltrados" 
        :key="produto.id" 
        class="receita-item" 
        @click="selecionarProduto(produto.id)"
        :class="{ 'selecionado': produto.id === produtoSelecionadoId }"
      >
        <h3>{{ produto.nome }}</h3>
        <img :src="produto.imagem || 'caminho/para/imagem/padrao.png'" :alt="produto.nome" class="receita-img">
        <p><strong>Categoria:</strong> {{ getNomeCategoria(produto.idCategoria) }}</p>
        <p><strong>Preço:</strong> R$ {{ produto.preco?.toFixed(2) }}</p>
        
        <div v-if="produto.id === produtoSelecionadoId" class="ingredientes-lista">
          <h4>Ingredientes:</h4>
          <ul v-if="getReceitaDoProduto(produto.id)?.itens.length > 0">
            <li v-for="item in getReceitaDoProduto(produto.id).itens" :key="item.ingredienteId">
              {{ getNomeIngrediente(item.ingredienteId) }} - ({{ item.quantidade }})
            </li>
          </ul>
          <p v-else>Este produto não possui uma receita cadastrada.</p>
        </div>

        <div class="botoes-acao">
          <button class="btn-editar-item">Editar</button>
          <button class="btn-excluir-item">Excluir</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { storeToRefs } from 'pinia'; // 1. Importe o storeToRefs
import CriaProduto from '../components/CriaProduto.vue';
import { useProdutosStore } from '../stores/Produtos';
import { useReceitasStore } from '../stores/receitas';
import { useCategoriasStore } from '../stores/categorias';
// 2. CORRIJA a importação da store de ingredientes
import { useIngredientesStore } from '../stores/ingredientes';

// Instanciando as stores
const produtosStore = useProdutosStore();
const receitasStore = useReceitasStore();
const categoriasStore = useCategoriasStore();
// 3. CORRIJA a instanciação da store de ingredientes
const ingredientesStore = useIngredientesStore();

// 4. Use storeToRefs para manter a reatividade dos dados da store
// Isso cria refs locais que apontam para o estado da store
const { produtos } = storeToRefs(produtosStore);
const { receitas } = storeToRefs(receitasStore);
const { categorias } = storeToRefs(categoriasStore);
const { ingredientes } = storeToRefs(ingredientesStore);

// State do componente
const showcriaNovoReceita = ref(false);
const loading = ref(false);
const error = ref(null);
const produtoSelecionadoId = ref(null);
const categoriaSelecionada = ref('');

// Carrega todos os dados necessários da API
onMounted(async () => {
  loading.value = true;
  error.value = null;
  try {
    // 5. As stores agora gerenciam seus próprios dados, não precisamos copiá-los
    await Promise.all([
      produtosStore.fetchProdutos(),
      receitasStore.fetchReceitas(),
      categoriasStore.fetchCategorias(),
      ingredientesStore.fetchIngredientes()
    ]);
  } catch (err) {
    error.value = "Erro ao carregar os dados. Tente novamente mais tarde.";
    console.error(err);
  } finally {
    loading.value = false;
  }
});

// Lógica para filtrar os produtos
const produtosFiltrados = computed(() => {
  if (!categoriaSelecionada.value) {
    // 6. Use 'produtos.value' (do storeToRefs) diretamente
    return produtos.value; 
  }
  return produtos.value.filter(p => p.idCategoria === categoriaSelecionada.value);
});

// Funções auxiliares (agora usando os refs reativos)
function getNomeCategoria(id) {
  const categoria = categorias.value.find(c => c.id === id);
  return categoria ? categoria.nome : 'Sem Categoria';
}

function getNomeIngrediente(id) {
  const ingrediente = ingredientes.value.find(i => i.id === id);
  return ingrediente ? ingrediente.nome : 'Ingrediente Desconhecido';
}

function getReceitaDoProduto(produtoId) {
  // 7. Acessa 'receitas.value' (do storeToRefs)
  return receitas.value.find(r => r.produtoId === produtoId);
}

function selecionarProduto(id) {
  if (produtoSelecionadoId.value === id) {
    produtoSelecionadoId.value = null;
  } else {
    produtoSelecionadoId.value = id;
  }
}
</script>

<style scoped>
/* (Mantenha todos os seus estilos .css aqui) */
/* Adicionei alguns estilos novos para a interatividade */
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

.info-receitas {
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

select {
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
  transition: all 0.3s ease;
}

select:focus {
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

.btn-novo-pedido {
  background-color: #ff7200;
}

.btn-novo-pedido:hover {
  background-color: #e65c00;
}

.card-receitas {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.receita-item {
  background: #f9f9f9;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 15px;
  box-shadow: 0 2px 4px rgb(0, 0, 0);
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.receita-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 6px 12px rgba(0,0,0,0.15);
}

.receita-item.selecionado {
  border-color: #ff7200;
  box-shadow: 0 0 10px rgba(255, 114, 0, 0.5);
}

.receita-img {
  width: 100%;
  height: 150px;
  object-fit: cover;
  border-radius: 4px;
  margin-bottom: 10px;
}

.ingredientes-lista {
  margin-top: 15px;
  padding-top: 10px;
  border-top: 1px solid #eee;
}

.ingredientes-lista ul {
  padding-left: 20px;
  margin: 5px 0 0 0;
}

.botoes-acao {
  margin-top: 15px;
  display: flex;
  gap: 10px;
}

.btn-editar-item {
  background-color: #007bff;
}

.btn-excluir-item {
  background-color: #dc3545;
}

.loading-message, .error-message {
  text-align: center;
  padding: 20px;
  font-size: 1.2rem;
}
</style>