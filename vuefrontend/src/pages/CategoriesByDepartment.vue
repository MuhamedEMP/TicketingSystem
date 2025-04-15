<template>
    <UserNavbar />
    <div class="overlay"></div>

    <div class="category-page">
      
      <h1>Categories for Department {{ deptName }}</h1>
      <!-- Admin-only: Add Category Button -->
    <div v-if="hasRole('admin')" class="button-container">
      <router-link
        :to="`/department/${deptIdFromRoute}/categories/add`"
        class="button small-button"
      >
        ➕ Add Category
      </router-link>
    </div>

    <ul v-if="categories.length">
      <li v-for="cat in categories" :key="cat.name" class="category-item">
        <div v-if="hasRole('admin')" style="display: inline-flex; gap: 0.5rem; align-items: center;">
          <button @click="deleteCategory(cat.id)" class="button small-button delete-button">
            🗑️ Delete
          </button>
        </div>

        <router-link
          :to="`/newticket/${deptIdFromRoute}/${cat.id}`"
          class="category-link"
        >
          {{ cat.name }} <span v-if="cat.description">- {{ cat.description }}</span>
        </router-link>
      </li>
  </ul>

  
      <p v-else>No categories for this department</p>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import { useRoute } from 'vue-router';
  import { getCategoriesByDepartment } from '../api/categoryApi';
  import { getDepartmentById } from '../api/departmentApi';
  import UserNavbar from '../components/UserNavbar.vue';
  import api from '../utils/api';

  const roles = JSON.parse(localStorage.getItem('roles') || '[]');
  const hasRole = (role) => roles.map(r => r.toLowerCase()).includes(role.toLowerCase());

  const route = useRoute();
  const deptIdFromRoute = route.params.deptId;
  
  const deptName = ref('');
  const categories = ref([]);


  const deleteCategory = async (categoryId) => {
  if (!confirm('Are you sure you want to delete this category?')) return

  try {
  await api.delete(`/admin/deletecategory/${categoryId}`)

  categories.value = categories.value.filter(cat => cat.id !== categoryId)
} catch (err) {
  if (err.response?.status === 409) {
    alert('❗ Cannot delete: This category is used by existing tickets.')
  } else {
    alert('Failed to delete category.')
  }
}

}

  
  onMounted(async () => {
    try {
      const dept = await getDepartmentById(deptIdFromRoute);
      deptName.value = dept.name;
  
      categories.value = await getCategoriesByDepartment(deptIdFromRoute);
    } catch (err) {
      console.error('Failed to load department or categories:', err);
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

.button {
  background-color: whitesmoke;
  border: 2px solid #232020;
  border-radius: 25px;
  font-size: 16px;
  font-weight: bold;
  color: #007bff;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 90px;
  text-align: center;
}

.small-button {
  padding: 8px 16px;
  font-size: 14px;
  border-radius: 20px;
  min-width: 70px;
}

  </style>
  