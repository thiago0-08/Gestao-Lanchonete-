import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(false);
  const token = ref(null);
  const username = ref(null); 

  function setAuthenticated(value) {
    isAuthenticated.value = value;
  }

  function setToken(newToken) {
    token.value = newToken;
  }

  function setUsername(newUsername) { 
    username.value = newUsername;
  }

  function logout() {
    isAuthenticated.value = false;
    token.value = null;
    username.value = null; 
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

