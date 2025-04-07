<template>
  <nav class="navbar">
    <div class="brand-title">eMedia</div>

    <div v-if="homePage">
      <div class="send-to-department">
        <h3 class="oneline">Send Ticket to:</h3>
        <div class="button-container">
          <button class="button active" id="button1">IT</button>
          <button class="button" id="button2">HR</button>
        </div>
      </div>
    </div>


    <div class="nav-links button-container">
      <router-link v-if="notHome" to="/home" class="button small-button">Home</router-link>
      <router-link to="/user/mytickets" class="button small-button">My Tickets</router-link>
      <button class="button small-button" @click="logout">Logout</button>
    </div>
  </nav>
</template>

<script setup>
import { useRouter, useRoute } from 'vue-router';
import { computed, ref } from 'vue';
import '../assets/js/custom.js';

const router = useRouter();
const route = useRoute();

const logout = () => {
  localStorage.clear();
  router.push('/');
};

const homePage = computed(() => route.path === '/home');
const notHome = computed(() => route.path !== '/home');

const search = ref('');

const applySearch = () => {
  if (myTicketsPage.value) {
    router.push({ path: '/user/mytickets', query: { search: search.value } });
  }
};
</script>

<style scoped>
@import '../assets/css/navbar.css';

@import '../assets/css/custom.css';
</style>



