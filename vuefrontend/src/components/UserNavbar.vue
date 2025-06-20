<template>
  <aside class="sidebar">
    <div class="sidebar-inner">
      
      <router-link to="/home" class="logo-link">
        <img src="../assets/eMediaLogo.png" alt="eMedia Patch Logo" class="sidebar-logo"/>
      </router-link>

      <div class="nav-icons">
      <button v-if="notMyTickets && hasPolicy('RegularUserOnly')" >
        <router-link to="/user/mytickets" class = "button-tickets">
          <i class="ai-clipboard"></i> 
        </router-link>
      </button>

      <button v-if="notMyResponses && hasPolicy('RegularUserOnly')" >
        <router-link to = "/myresponses" class = "button-tickets">
          <i class="ai-chat-approve"></i>
        </router-link>
      </button>
      
      <button>
        <router-link to = "/profile" class = "button-tickets">
          <i class="ai-person"></i>
        </router-link>
      </button>
      </div>

      <!-- ✅ Show to admins OR department users -->
      <button>
        <router-link v-if="hasPolicy('DepartmentUserOnly') && !isOnSharedTickets" to="/sharedtickets" class = "button-tickets">
          <i class="ai-clipboard"></i> 
        </router-link>
      </button>

      <!-- ✅ Show to admins OR department users -->
      <button>
        <router-link v-if="hasPolicy('DepartmentUserOnly') && !isOnSharedTickets" to="/mydepartments" class = "button-tickets">
          <i class="ai-people-group"></i>
        </router-link>
      </button>

      <!-- ✅ Show to department users -->
      <button>
        <router-link v-if="hasPolicy('DepartmentUserOnly') && !isOnSharedTickets" to="/sentresponses" class = "button-tickets">
          <i class="ai-envelope"></i>
        </router-link>
      </button>

      <div class="left-logout">
        <button @click="logout">
          <i class="ai-sign-out"></i>
        </button>
      </div>



    
      
    </div>
  </aside>
</template>
<script setup>
import {  ref } from 'vue';
import { useRouteFlags } from '../utils/routeUtils';
import router from '../router';
import { hasPolicy } from '../utils/hasPolicy';
import { useRoute } from 'vue-router';

const route = useRoute();
const { notHome, notMyTickets } = useRouteFlags();

const isOnSharedTickets = route.path === '/sharedtickets';
const isOnSentResponses = route.path === '/sentresponses';
const isOnMyDepartments = route.path === '/mydepartments';
const notMyResponses = route.path != '/myresponses';

const dropdownOpen = ref(false);

const toggleDropdown = () => {
  dropdownOpen.value = !dropdownOpen.value;
};

const logout = () => {
  localStorage.clear();
  router.push('/');
};


</script>



<style scoped>

@import '../assets/css/sidebar.css';

</style>