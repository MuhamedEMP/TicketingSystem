<template>
  <div>
    <UserNavbar />
    <div class="overlay"></div>

    <!-- 👤 Regular Users (no admin or department access) -->
    <div v-if="hasPolicy('RegularUserOnly')" class="home-page">
      <div class="welcome-box">
        <img src="../assets/user-icon.png" alt="User Avatar" class="avatar" />
        <h2>Welcome, <strong>{{ firstName || 'User' }}</strong>!</h2>
      </div>

      <div v-if="departments.length" class="department-container">
        <div
          v-for="dept in departments"
          :key="dept.id"
          class="department-card"
        >
          <router-link
            :to="`/department/${dept.id}/categories`"
            class="department-link"
          >
            <div class="plus-icon">+</div>
            <h2>Select<br>an {{ dept.name }} ticket category</h2>
            <p>Request {{ dept.name === 'IT' ? 'a new tool or hardware for yourself or new hires. Report an issue with your device or software.' : 'confirmation or paperwork. Submit sick leave documentation.' }}</p>
          </router-link>
        </div>
      </div>
      <p v-else>Loading departments and categories...</p>
    </div>

    <!-- 🏢 Department Users -->
    <div v-if="hasPolicy('DepartmentUserOnly')" class="home-page">
      <div class="welcome-box">
        <img src="../assets/user-icon.png" alt="User Avatar" class="avatar" />
        <h2>Welcome, <strong>{{ firstName || 'User' }}</strong>!</h2>
      </div>
      <p>You can view and respond to tickets assigned to your department.</p>
      <router-link to="/mydepartments" class="button">Go to My Departments</router-link>
    </div>

    <!-- 🛠️ Admin Users -->
    <div v-if="hasPolicy('AdminOnly')" class="home-page">
      <h1>Welcome, {{ firstName }}</h1>
      <p>You have access to full system controls and user management.</p>
      <router-link to="/admin" class="button">Go to Admin Panel</router-link>
    </div>

    <!-- 🧑‍💼 Admin OR Dept User -->
    <div v-if="hasPolicy('AdminAndDepartmentUser')" class="home-page">
      <h1>Welcome, {{ firstName }}</h1>
      <h2>Shared Ticket View</h2>
      <p>You have admin role and department user roles. <br></p>
        <router-link to="/admin" class="button">Go to Admin Panel</router-link> 
        <br>     
      <router-link to="/mydepartments" class="button">Go to My Departments</router-link>
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

const firstName = localStorage.getItem("firstName");

const categoriesByDepartment = (deptId) => {
  return Array.isArray(categories.value)
    ? categories.value.filter(cat => cat?.departmentId === deptId)
    : [];
};


onMounted(async () => {
  try {
    const [depts, cats] = await Promise.all([
      getAllDepartments(),
      getAllCategories()
    ]);

    console.log("✅ Departments:", depts);
    console.log("✅ Categories:", cats);

    departments.value = depts || [];
    categories.value = Array.isArray(cats) ? cats : [];
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
  background-color: #ca0176;
  color: white;
  border: none;
  border-radius: 8px;
  text-decoration: none;
  font-weight: bold;
}
.button:hover {
  color: #ca0176;
  background-color: white;
}

.welcome-box {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.welcome-box h2 {
  font-size: 2rem;
  font-weight: 500;
  color: white;
}
.welcome-box strong {
  font-weight: 800;
}

.welcome-wrapper {
  display: flex;
  align-items: center;
  gap: 2rem;
  background: radial-gradient(circle at top left, #ffffff11, #ffffff05);
  padding: 2rem 3rem;
  border-radius: 20px;
  margin: 2rem auto;
  max-width: 900px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
}

.welcome-right h1 {
  font-size: 2rem;
  color: white;
  font-weight: 500;
  line-height: 1.3;
}
.welcome-right strong {
  font-weight: 800;
}

.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: #ccc;
}

.department-card {
  width: 300px;
  height: 360px;
  background: white;
  color: black;
  border-radius: 16px;
  padding: 2rem;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
  align-items: flex-start;
  position: relative;
  transition: transform 0.2s ease;
}

.department-card:hover {
  transform: translateY(-4px);
}

.home-page {
  padding: 2rem;
  color: #eee;
  text-align: center;
}

.department-container {
  display: flex;
  justify-content: center;
  gap: 2.5rem; /* or 3rem */
  flex-wrap: wrap;
  margin-top: 2rem;
}

.department-block {
  width: 45%;
  background-color: #2a2a2a;
  border-radius: 10px;
  padding: 2rem;
  min-height: 400px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-sizing: border-box;
  text-align: center;
}

.department-block ul {
  list-style: none;
  padding-left: 0;
}

.department-block h2 {
  font-size: 2rem;
}

@media (max-width: 786px) {
  .department-block {
    flex: 1 1 100%;
    max-width: 100%
  }
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

.dep {
  color: #fff;
  text-decoration: none;
}

.department-link {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  text-align: left;
  justify-content: space-between;
  height: 100%;
  width: 100%;
  text-decoration: none;
  color: inherit;
  padding-top: 2rem; /* optional: if you want distance from + icon */
  gap: 1.5rem; /* ← adds vertical spacing between +, heading, and paragraph */
}


.department-link h2 {
  font-size: 1.6rem;
  font-weight: 800;
  margin: 0;
  line-height: 1.4;
}

.department-link h2:hover {
  color: #ca0176;
  text-decoration: none;
}

.department-link p {
  font-size: 0.95rem;
  color: #444;
  margin: 0;
}

.category-link {
  color: #ccc;
  text-decoration: none;
  font-weight: 500;
}

.category-link:hover {
  text-decoration: none;
  color:rgb(255, 255, 255);
}

.plus-icon {
  position: absolute;
  top: 1rem;
  left: 1rem;
  background: black;
  color: white;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  font-weight: bold;
}

</style>
