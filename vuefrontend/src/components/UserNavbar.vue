<template>
  <nav class="navbar">
    <div class="brand-title">eMedia</div>

    <div v-if="homePage">
      <div class="send-to-department">
        <h3 class="oneline">Send Ticket to:</h3>
        <div class="button-container">
  <button
    class="button"
    :class="{ active: activeDepartmentId === 1 }"
    @click="setDepartment(1)"
  >
    IT
  </button>

  <button
    class="button"
    :class="{ active: activeDepartmentId === 2 }"
    @click="setDepartment(2)"
  >
    HR
  </button>
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

const activeDepartmentId = ref(null);

const setDepartment = (id) => {
  activeDepartmentId.value = id;
  // Emit to parent
  emit('select-department', id);
};

// You need this to emit properly
const emit = defineEmits(['select-department']);

import { onMounted } from 'vue';

onMounted(() => {
  setDepartment(1); // 👈 auto-select IT on mount
});

</script>

<style scoped>
@import '../assets/css/navbar.css';

@import '../assets/css/custom.css';
</style>



