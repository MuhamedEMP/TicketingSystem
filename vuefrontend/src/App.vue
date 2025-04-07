<!-- src/App.vue -->
<script setup>
import { useRoute } from 'vue-router';
import Navbar from './components/UserNavbar.vue';
import { computed } from 'vue';

const route = useRoute();

const showNavbar = computed(() => route.path !== '/');
</script>

<template>
  <div>
    <Navbar v-if="showNavbar" />
    <router-view />
  </div>
</template>
