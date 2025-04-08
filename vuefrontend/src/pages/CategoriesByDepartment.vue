<template>
    <UserNavbar />
    <div class="overlay"></div>

    <div class="category-page">
      
      <h1>Categories for Department {{ deptId }}</h1>
  
      <ul v-if="categories.length">
        <li v-for="cat in categories" :key="cat.name">
                <router-link
                    :to="`/newticket/${deptId}/${cat.id}`"
                    class="category-link"
                >
                    {{ cat.name }} <span v-if="cat.description">- {{ cat.description }}</span>
                </router-link>
        </li>

      </ul>
  
      <p v-else>Loading categories...</p>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import { getCategoriesByDepartment } from '../api/categoryApi';
  import UserNavbar from '../components/UserNavbar.vue';
  
  const route = useRoute();
  const deptId = route.params.deptId;
  
  const categories = ref([]);
  
  onMounted(async () => {
    try {
      categories.value = await getCategoriesByDepartment(deptId);
    } catch (err) {
      console.error('Failed to load categories:', err);
    }
  });
  </script>
  
  <style scoped>
  .category-page {
    padding: 2rem;
    color: #eee;
  }
  
  li {
    margin: 0.5rem 0;
  }
  .department-link {
  text-decoration: none;
  color: inherit;
  cursor: pointer;
}

.department-link h2:hover {
  text-decoration: underline;
  color: #42b983;
}
  </style>
  