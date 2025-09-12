import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(false);
  const token = ref(null);

  function setAuthenticated(value) {
    isAuthenticated.value = value;
  }

  function setToken(newToken) {
    token.value = newToken;
  }

  function logout() {
    isAuthenticated.value = false;
    token.value = null;
  }

  return { isAuthenticated, token, setAuthenticated, setToken, logout };
});