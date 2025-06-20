<template>
  <UserNavbar />
  <div class="overlay"></div>

  <div class="category-page">
    <h1>Categories for Department {{ deptName }}</h1>

    <!-- Admin Add Category Button -->
    <div v-if="hasPolicy('AdminOnly')" class="button-container">
      <router-link
        :to="`/department/${deptIdFromRoute}/categories/add`"
        class="button small-button"
      >
        ➕ Add Category
      </router-link>
    </div>

    <!-- Cards -->
    <div v-if="categories.length" class="category-grid">
      <div
        v-for="cat in categories"
        :key="cat.id"
        class="category-card"
      >
        <router-link
          :to="`/newticket/${deptIdFromRoute}/${cat.id}`"
          class="card-link"
          :class="{ 'disabled-link': !isRegularUser }"
          @click.prevent="!isRegularUser"
        >
          <div class="plus-circle">+</div>
          <h3>{{ cat.name }}</h3>
          <p class="card-description">
            {{ categoryDescriptions[cat.name] || 'Submit a request or ask for support.' }}
          </p>
        </router-link>

        <button
          v-if="hasPolicy('AdminOnly') || hasPolicy('AdminAndDepartmentUser') || hasPolicy('AdminOrDepartmentUser')"
          @click="deleteCategory(cat.id)"
          class="button small-button delete-button"
        >
          🗑️
        </button>
      </div>
    </div>

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
import { hasPolicy } from '../utils/hasPolicy';

const route = useRoute();
const deptIdFromRoute = route.params.deptId;

const deptName = ref('');
const categories = ref([]);

const categoryDescriptions = {
  Onboarding: "Request a laptop, peripherals, and email setup for a new hire.",
  Offboarding: "Notify the IT team when a team member is departing and arrange equipment return.",
  "Request Hardware": "Need a mouse, keyboard, stand, or other accessories? Submit a request.",
  "Request Sofware": "Request a software installation.",
  "Report an Issue": "Report any issue you're experiencing.",
  Other: "Submit any other request here.",
  "Employment Paperwork": "Submit a request for Proof of Employment or ask for assistance.",
  "Sick Leave Documentation": "Upload a photo of the Sick Leave Form.",
  "Other Paperwork": "Submit any other paperwork request or ask for assistance."
};

const deleteCategory = async (categoryId) => {
  if (!confirm('Are you sure you want to delete this category?')) return;

  try {
    await api.delete(`/admin/deletecategory/${categoryId}`);
    categories.value = categories.value.filter(cat => cat.id !== categoryId);
  } catch (err) {
    if (err.response?.status === 409) {
      alert('❗ Cannot delete: This category is used by existing tickets.');
    } else {
      alert('Failed to delete category.');
    }
  }
};

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
  background: rgba(255, 255, 255, 0.06);
  backdrop-filter: blur(8px);
  border-radius: 24px;
  padding: 2rem;
  margin: 2rem;
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.2);
  color: #eee;
}

h1 {
  text-align: center;
}

.button-container {
  display: flex;
  justify-content: center;
  margin-top: 1rem;
}

.category-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 2rem;
  justify-items: center;
  margin-top: 2rem;
}

.category-card {
  background: white;
  border-radius: 16px;
  padding: 1.5rem;
  width: 240px;
  height: 200px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
  position: relative;
  transition: transform 0.2s ease;
}

.category-card:hover {
  transform: translateY(-4px);
}

.card-link {
  text-decoration: none;
  color: black;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.card-link h3 {
  margin: 2rem 0 0.5rem;
  font-size: 1.2rem;
  font-weight: 700;
  line-height: 1.3;
}

.card-description {
  font-size: 0.95rem;
  color: #444;
  margin-top: auto;
  line-height: 1.4;
}

.plus-circle {
  background: black;
  color: white;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 16px;
  position: absolute;
  top: 12px;
  left: 12px;
}

.button {
  text-decoration: none;
  background-color: whitesmoke;
  border: 2px solid #232020;
  border-radius: 25px;
  font-size: 16px;
  font-weight: bold;
  color: black;
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

.delete-button {
  align-self: flex-end;
  margin-top: 0.5rem;
}
</style>
