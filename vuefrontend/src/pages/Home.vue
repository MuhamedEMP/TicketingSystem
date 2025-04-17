<template>
  <div>
    <UserNavbar />
    <div class="overlay"></div>

    <!-- 👤 Regular Users (no admin or department access) -->
    <div v-if="hasPolicy('RegularUserOnly')" class="home-page">
      <h1>Send New Ticket</h1>

      <div v-if="departments.length && categories.length">
        <div
          v-for="dept in departments"
          :key="dept.id"
          class="department-block"
        >
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

    <!-- 🏢 Department Users -->
    <div v-if="hasPolicy('DepartmentUserOnly')" class="home-page">
      <h1>Welcome, Department Staff</h1>
      <p>You can view and respond to tickets assigned to your department.</p>
      <router-link to="/sharedtickets" class="button">Go to Department Tickets</router-link>
    </div>

    <!-- 🛠️ Admin Users -->
    <div v-if="hasPolicy('AdminOnly')" class="home-page">
      <h1>Welcome, Admin</h1>
      <p>You have access to full system controls and user management.</p>
      <router-link to="/admin" class="button">Go to Admin Panel</router-link>
    </div>

    <!-- 🧑‍💼 Admin OR Dept User -->
    <div v-if="hasPolicy('AdminOrDepartmentUser')" class="home-page">
      <h2>Shared Ticket View</h2>
      <p>You have access to tickets relevant to your departments or all tickets if you're an admin.</p>
      <router-link to="/sharedtickets" class="button">View Tickets</router-link>
    </div>
  </div>
</template>



<script setup>
import UserNavbar from '../components/UserNavbar.vue';
import { ref, onMounted } from 'vue';
import { getAllDepartments } from '../api/departmentApi';
import { getAllCategories } from '../api/categoryApi';
import { hasPolicy } from '../utils/hasPolicy';

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

.button {
  display: inline-block;
  margin-top: 1rem;
  padding: 0.6rem 1.2rem;
  background-color: #42b983;
  color: white;
  border: none;
  border-radius: 8px;
  text-decoration: none;
  font-weight: bold;
}
.button:hover {
  background-color: #36966e;
}

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
