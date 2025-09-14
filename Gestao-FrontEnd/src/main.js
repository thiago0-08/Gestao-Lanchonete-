import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import router from './router';
import axios from 'axios';

// Pega o token do localStorage
const token = localStorage.getItem('authToken');

// configura o header de autorização globalmente
if (token) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
}

const app = createApp(App);

app.use(createPinia());
app.use(router);

app.mount('#app');