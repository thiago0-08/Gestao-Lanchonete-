import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  // Inicializa o estado lendo o token do localStorage
  const token = ref(localStorage.getItem('authToken'));
  // O estado de autenticação depende da existência do token
  const isAuthenticated = ref(!!token.value);
  
  const username = ref(null);

  function setAuthenticated(value) {
    isAuthenticated.value = value;
  }

  function setToken(newToken) {
    token.value = newToken;
    // O token é salvo no localStorage no momento do login.
    // Aqui, a função apenas atualiza a store.
  }

  function setUsername(newUsername) {
    username.value = newUsername;
  }

  function logout() {
    isAuthenticated.value = false;
    token.value = null;
    username.value = null;
    // Remove o token do localStorage
    localStorage.removeItem('authToken');
  }

  return { isAuthenticated, token, username, setAuthenticated, setToken, setUsername, logout };
});

export const aletaEstoque = defineStore('alertaEstoque', () => {
  const alertaEstoque = ref(false);
  function setAlertaEstoque(value) {
    alertaEstoque.value = value;
  }
  return { alertaEstoque, setAlertaEstoque };
});