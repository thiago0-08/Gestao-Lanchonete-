<template>
  <div id="app">
    <NavBar v-if="$route.name !== 'Login'" />
    <RouterView />
  </div>
</template>

<script>
import NavBar from './components/NavBar.vue';
import { RouterView } from 'vue-router';


export default {
  components: {
    NavBar,
    RouterView,
  },
};
</script>
