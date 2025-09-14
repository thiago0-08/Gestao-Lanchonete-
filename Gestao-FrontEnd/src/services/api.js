import axios from 'axios';

// Cria uma instância do Axios para a API
const api = axios.create({
  baseURL: 'http://localhost:5138/api', // URL base da sua API
});

// Intercepta todas as requisições para adicionar o token de autorização
api.interceptors.request.use(config => {
  // Pega o token do localStorage
  const token = localStorage.getItem('authToken');

  // Se o token existir, adiciona-o ao cabeçalho Authorization
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
}, error => {
  return Promise.reject(error);
});

export default api;