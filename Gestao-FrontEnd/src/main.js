import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';
import router from './router';
import axios from 'axios';


import { library } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faEye, faPencilAlt, faTrashAlt } from '@fortawesome/free-solid-svg-icons';


library.add(faEye, faPencilAlt, faTrashAlt);

const token = localStorage.getItem('authToken');

if (token) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
}

const app = createApp(App);

app.use(createPinia());
app.use(router);


app.component('font-awesome-icon', FontAwesomeIcon);

app.mount('#app');