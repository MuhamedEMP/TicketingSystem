<script setup>
import { useRoute } from 'vue-router';
import { computed } from 'vue';
import Sidebar from './components/UserNavbar.vue';

const route = useRoute();
const hiddenSidebarRoutes = ['/'];
const showSidebar = computed(() => !hiddenSidebarRoutes.includes(route.path));
</script>

<template>
  <div class="background-wrapper">
    <div class="app-layout">
      <Sidebar v-if="showSidebar" />
      <div
        class="content-wrapper"
        :style="{ marginLeft: showSidebar ? '80px' : '0px' }"
      >
        <router-view />
      </div>
    </div>
  </div>
</template>

<style>
/* 🔥 Set global background */
.background-wrapper {
  min-height: 100vh;
  background-image: url('assets/eMP_BG.jpg');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  background-attachment: fixed;
}

/* 🔧 Your layout */
.app-layout {
  display: flex;
}

/* 📄 Adjust content offset */
.content-wrapper {
  margin-left: 120px;
  padding: 24px;
  transition: margin-left 0.3s ease;
  width: 100%;
}

/* 🎯 Login screen style (if used) */
.login-layout {
  width: 100%;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
