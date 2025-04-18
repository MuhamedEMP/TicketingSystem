<template>
    <div class="my-departments-page">
      <UserNavbar />
      
    <div class="overlay"></div>
      <h1>Departments You Can Access</h1>
  
      <div v-if="departments.length && categories.length">
        <div
          v-for="dept in departments"
          :key="dept.id"
          class="department-block"
        >
        <router-link :to="`/sharedtickets?departmentName=${dept.name}`" class="department-link">
            <h2>{{ dept.name }}</h2>
        </router-link>

          <p v-if="dept.description" class="dept-desc">{{ dept.description }}</p>
  
          <ul>
            <li
              v-for="cat in categoriesByDepartment(dept.id)"
              :key="cat.id"
              class="category-item"
            >
              {{ cat.name }}
              <span v-if="cat.description"> - {{ cat.description }}</span>
            </li>
          </ul>
        </div>
      </div>
  
      <p v-else>Loading your departments and categories...</p>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import UserNavbar from '../../components/UserNavbar.vue';
  import { getMyAssignedDepartments } from '../../api/departmentApi';
  import { getAllCategories } from '../../api/categoryApi';
  
  const departments = ref([]);
  const categories = ref([]);
  
  const categoriesByDepartment = (deptId) => {
    return categories.value.filter(cat => cat.departmentId === deptId);
  };
  
  onMounted(async () => {
    try {
      departments.value = await getMyAssignedDepartments();
      categories.value = await getAllCategories();
    } catch (error) {
      console.error('❌ Failed to load departments or categories:', error);
    }
  });
  </script>
  
  <style scoped>

  .department-block {
    background-color: #2a2a2a;
    padding: 1rem;
    margin-bottom: 2rem;
    border-radius: 10px;
  }
  
  .department-block h2 {
    color: #fff;
    margin-bottom: 0.5rem;
  }
  
  .dept-desc {
    color: #ccc;
    margin-bottom: 0.5rem;
  }
  
  .category-item {
    margin-left: 1rem;
    padding: 0.4rem;
    color: #ccc;
  }
  </style>
  