import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth';
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
      name: 'login',
      component: () => import('../Login.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/Dashboard',
      name: 'dashboard',
      component: () => import('../views/Dashboard.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/Pedidos',
      name: 'pedidos',
      component: () => import('../views/Pedidos.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/Estoque',
      name: 'estoque',
      component: () => import('../views/Estoque.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/Receitas',
      name: 'receitas',
      component: () => import('../views/Receitas.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/Relatorio',
      name: 'relatorio',
      component: () => import('../views/Relatorio.vue'),
      meta: { requiresAuth: true }
    },
     

  ],
});

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore(); // Inicialize a store

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login' });
  } else {
    next();
  }
});

export default router

