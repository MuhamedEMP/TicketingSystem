<template>
  <div>
    <div class="overlay"></div>
    <UserNavbar @select-department="selectDepartment" />

    <div v-if="selectedDepartmentId && departmentCategories.length > 0" class="category-section">
  <h2>Categories for Department {{ selectedDepartmentId }}</h2>
  <ul class="category-list">
    <li v-for="cat in departmentCategories" :key="cat.id">
      {{ cat.name }}
    </li>
  </ul>
</div>



    <!-- Dynamic content injection area -->
    <div class="popup-container"></div>
    <div class="page" id="page">
      <div class="page-content" id="pageContent"></div>
    </div>
  </div>
</template>


<style scoped>
/* @import '../assets/css/custom.css'; */
/* Include your custom styles */
</style>


<script setup>
import UserNavbar from '../components/UserNavbar.vue';
import { ref, onMounted } from 'vue';
import { getAllCategories } from '../api/categoryApi';
import { getAllDepartments } from '../api/departmentApi';

const allCategories = ref([]);
const selectedDepartmentId = ref(null); // update this on button click
const departmentCategories = ref([]);
const departments = ref([]);


// 
const selectDepartment = (id) => {
  selectedDepartmentId.value = id;

  // 👇 use the line you mentioned to filter categories
  departmentCategories.value = allCategories.value.filter(
  (cat) => cat.departmentId === selectedDepartmentId.value
);

};

// onMounted(loadCategories);
onMounted(async () => {
  try {
    
    departments.value = await getAllDepartments();
    allCategories.value = await getAllCategories();
    selectDepartment(1); // optional: auto-select first department
  } catch (error) {
    console.error('Failed to load data:', error);
  }
});

</script>
