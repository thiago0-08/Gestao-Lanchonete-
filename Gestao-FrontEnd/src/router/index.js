import { createRouter, createWebHistory } from 'vue-router'
import dashboard from '../views/Dashboard.vue'
import pedidos from '../views/Pedidos.vue'
import estoque from '../views/Estoque.vue'
import receitas from '../views/Receitas.vue'
import relatorio from '../views/Relatorio.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: () => import('../views/Dashboard.vue'),
    },
    {
      path: '/Pedidos',
      name: 'pedidos',
      component: () => import('../views/Pedidos.vue'),
    },
    {
      path: '/Estoque',
      name: 'estoque',
      component: () => import('../views/Estoque.vue'),
    },
    {
      path: '/Receitas',
      name: 'receitas',
      component: () => import('../views/Receitas.vue'),
    },
    {
      path: '/Relatorio',
      name: 'relatorio',
      component: () => import('../views/Relatorio.vue'),
    },
  ],
})

export default router
