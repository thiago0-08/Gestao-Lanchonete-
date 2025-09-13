<template>
    <div class="modal-overlay" @click.self="$emit('close')">
        <div class="modal-container">
            <h2 class="titulo">Saida de Produto</h2>
            <button class="btn-fechar" @click="$emit('close')">X</button>

            <!-- Mensagem de sucesso -->
            <div v-if="message" class="success-message">
                {{ message }}
            </div>

            <form @submit.prevent="submitTicket">
                <div class="form-group">
                    <label for="cliente">Nome do produto:</label>
                    <input type="text" id="cliente" placeholder="Ex: Hambúrguer" v-model="produtoId" required>
                </div>

                <div class="form-group">
                    <label for="data-pedido">Quantidade de saida</label>
                    <input type="number" id="quantidade-saida" v-model="quantidade" placeholder="Ex: 5" step="0.01" required>
                </div>

                
                
                <button type="submit" class="btn-enviar">Enviar</button>
            </form>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { entradaSaidaEstoque } from '../../stores/EntradaSaidaEstoque.js';


const produtoId = ref('');
const quantidade = ref(0);
const message = ref('');


const store = entradaSaidaEstoque();


async function submitTicket() {
 
  const payload = {
    produtoId: produtoId.value,
    quantidade: quantidade.value,
    tipo: 'saida'
  };

  try {
    
    await store.addEntradaSaida(payload);

    
    message.value = 'Saida do Produto feita com sucesso!';
    
    
    produtoId.value = '';
    quantidade.value = 0;

    
    setTimeout(() => {
      message.value = '';
      emit('close'); 
    }, 3000);

  } catch (error) {
    
    message.value = 'Erro ao fazer a saida do produto.';
    console.error('Erro:', error);
  }
}


const emit = defineEmits(['close']);
</script>

<style scoped>
/* Fundo escuro do modal */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

/* Container principal do modal */
.modal-container {
    background-color: #fff;
    padding: 30px;
    border-radius: 8px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    width: 90%;
    max-width: 500px;
    position: relative;
}

.titulo {
    text-align: center;
    color: #333;
    margin-bottom: 20px;
}

/* Botão de fechar (X) */
.btn-fechar {
    position: absolute;
    top: 15px;
    right: 15px;
    background: none;
    border: none;
    font-size: 1.2rem;
    cursor: pointer;
    color: #888;
    transition: color 0.3s;
}
.btn-fechar:hover {
    color: #333;
}

/* Grupos de formulário */
.form-group {
    margin-bottom: 15px;
}

.form-group label {
    display: block;
    margin-bottom: 5px;
    font-weight: bold;
    color: #555;
}

.form-group input,
.form-group textarea {
    width: 100%;
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 1rem;
    box-sizing: border-box;
    transition: border-color 0.3s, box-shadow 0.3s;
}

.form-group input:focus,
.form-group textarea:focus {
    border-color: #ff7200;
    box-shadow: 0 0 5px rgba(255, 114, 0, 0.5);
    outline: none;
}

/* Botão Enviar */
.btn-enviar {
    display: block;
    width: 100%;
    padding: 12px;
    background-color: #ff7200;
    color: white;
    border: none;
    border-radius: 4px;
    font-size: 1.1rem;
    cursor: pointer;
    margin-top: 15px;
    transition: background-color 0.3s;
}
.btn-enviar:hover {
    background-color: #e65c00;
}

/* Estilo da mensagem de sucesso */
.success-message {
    text-align: center;
    padding: 15px;
    margin-bottom: 20px;
    background-color: #d4edda;
    color: #e65c00;
    border: 1px solid #c3e6cb;
    border-radius: 4px;
    font-weight: bold;
}
</style>
