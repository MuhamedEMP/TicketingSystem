<template>
  <div class="my-departments-page">
    <UserNavbar />

    <div class="overlay"></div>
    <h1 class="head">Departments You Can Access</h1>

    <div v-if="departments.length && categories.length">
      <div
        v-for="dept in departments"
        :key="dept.id"
        class="department-block"
      >
        <router-link
          :to="`/sharedtickets?departmentName=${dept.name}`"
          class="department-link department-link-v2"
        >
          <h2>{{ dept.name }}</h2>
        </router-link>

        <p v-if="dept.description" class="dept-desc">{{ dept.description }}</p>

        <ul>
          <li
            v-for="cat in categoriesByDepartment(dept.id)"
            :key="cat.id"
            class="category-item"
          >
            <router-link
              :to="`/sharedtickets?departmentName=${dept.name}&categoryName=${cat.name}`"
              class="category-link category-link-v2"
            >
              {{ cat.name }}
              <span v-if="cat.description"> - {{ cat.description }}</span>
            </router-link>
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
.my-departments-page {
  padding: 2rem;
  position: relative;
  z-index: 0;
  min-height: 100vh;
}

.overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(to right, #1a1a1a, #ff6600);
  z-index: -1;
}

.head {
  color: white;
  margin-bottom: 2rem;
}

.department-block {
  background-color: white;
  color: black;
  padding: 1rem 1.5rem;
  margin-bottom: 2rem;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.department-block h2 {
  color: black;
  text-decoration: none;
  margin-bottom: 0.5rem;
}

.dept-desc {
  color: #666;
  margin-bottom: 0.5rem;
}

.category-item {
  margin-left: 1rem;
  padding: 0.4rem;
  color: #444;
  list-style-type: disc;
}

.category-link-v2 {
  color: black;
  text-decoration: none;
}

.category-link-v2:hover {
  color: #ca0176;
}

.department-block h2:hover {
  color: #ca0176;
}

.department-link-v2 {
  text-decoration: none;
}
</style>
