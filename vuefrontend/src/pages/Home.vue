<template>
  <div>
    <UserNavbar />
    <div class="overlay"></div>

    <div class="home-page">
      <h1>Send New Ticket</h1>

      <div v-if="departments.length && categories.length">
        <div
          v-for="dept in departments"
          :key="dept.id"
          class="department-block"
        >
          <!-- 🔗 Department name is a link -->
          <router-link
            :to="`/department/${dept.id}/categories`"
            class="department-link"
          >
            <h2>{{ dept.name }}</h2>
          </router-link>

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

      <p v-else>Loading departments and categories...</p>
    </div>
  </div>
</template>


<script setup>
import UserNavbar from '../components/UserNavbar.vue';
import { ref, onMounted } from 'vue';
import { getAllDepartments } from '../api/departmentApi';
import { getAllCategories } from '../api/categoryApi';

const departments = ref([]);
const categories = ref([]);

const categoriesByDepartment = (deptId) => {
  return categories.value.filter(cat => cat.departmentId === deptId);
};

onMounted(async () => {
  try {
    departments.value = await getAllDepartments();
    categories.value = await getAllCategories();
  } catch (error) {
    console.error('❌ Failed to load departments or categories:', error);
  }
});
</script>

<style scoped>
.home-page {
  padding: 2rem;
  color: #eee;
}

.department-block {
  margin-bottom: 2rem;
  background-color: #2a2a2a;
  padding: 1rem;
  border-radius: 10px;
}

.department-block h2 {
  margin-bottom: 0.5rem;
  color: #fff;
}

.category-item {
  margin-left: 1rem;
  padding: 0.4rem;
  color: #ccc;
}

.department-link h2 {
  margin-bottom: 0.5rem;
  color: #fff;
  text-decoration: none;
}

.department-link h2:hover {
  color: #42b983;
  text-decoration: underline;
}

.category-link {
  color: #ccc;
  text-decoration: none;
  font-weight: 500;
}

.category-link:hover {
  text-decoration: underline;
  color: #42b983;
}

</style>
