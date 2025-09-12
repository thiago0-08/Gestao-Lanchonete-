<template>
  <div id="app">
    <!-- O componente NavBar só é renderizado se a rota atual não for a de Login -->
    <NavBar v-if="$route.name !== 'Login'" />
    <RouterView />
  </div>
</template>

<script>
import NavBar from '@/components/NavBar.vue';

export default {
  components: {
    NavBar,
  }
};
</script>
