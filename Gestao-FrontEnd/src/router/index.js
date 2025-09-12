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
     {
      path: '/',
      name: 'login',
      component: () => import('../Login.vue'),
      meta: { requiresAuth: false }
    },

  ],
});

router.beforeEach((to, from, next) => {
  // Lógica de verificação de autenticação:
  // Substitua a linha abaixo pela sua lógica real. Por exemplo, verifique se há um token de login no localStorage.
  const isAuthenticated = false; // Mude para true quando o usuário estiver logado.

  if (to.meta.requiresAuth && !isAuthenticated) {
    // Se a rota requer autenticação e o usuário não está logado,
    // redireciona para a tela de login.
    next({ name: 'Login' });
  } else {
    // Caso contrário, continua a navegação.
    next();
  }
});

export default router

