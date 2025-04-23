<template>
    <div>
      <AdminNavbar />
      <div class="overlay"></div>
  
      <div class="home-page">
        <h1>Existing Departments and Categories</h1>
  
        <div>
          <h2><RouterLink to="/admin/adddepartment">Add Department</RouterLink></h2>
        </div>
  
        <div v-if="departments.length">
          <div
            v-for="dept in departments"
            :key="dept.id"
            class="department-block"
          >
            <div style="display: flex; justify-content: space-between; align-items: center;">
              <router-link
                :to="`/department/${dept.id}/categories`"
                class="department-link"
              >
                <h2>{{ dept.name }}</h2>
              </router-link>
  
              <!-- Add Category Button -->
              <router-link
                :to="`/department/${dept.id}/categories/add`"
                class="button small-button"
              >
                ➕ Add Category
              </router-link>
              <!-- Delete Department Button -->
                <button
                @click="deleteDepartment(dept.id)"
                class="button small-button delete-dept-button"
                >
                🗑️ Delete Department
                </button>

            </div>
  
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
            <p v-if="errorMessageByDept[dept.id]" class="delete-error">
  {{ errorMessageByDept[dept.id] }}
</p>

          </div>
        </div>
  
        <p v-else>Loading departments and categories...</p>
      </div>
    </div>
  </template>
  
  
  
  <script setup>
import AdminNavbar from '../../components/adminComponents/AdminNavbar.vue'
import { ref, onMounted } from 'vue'
import { getAllDepartments } from '../../api/departmentApi'
import { getAllCategories } from '../../api/categoryApi'
import api from '../../utils/api'
const departments = ref([])
const categories = ref([])
const errorMessageByDept = ref({}) // 👈 store errors per dept

const categoriesByDepartment = (deptId) => {
  return categories.value.filter(cat => cat.departmentId === deptId)
}

const loadData = async () => {
  try {
    departments.value = await getAllDepartments()
    categories.value = await getAllCategories()
  } catch (error) {
    console.error('❌ Failed to load departments or categories:', error)
  }
}

onMounted(loadData)

const deleteDepartment = async (deptId) => {
  if (!confirm('Are you sure you want to delete this department?')) return

  errorMessageByDept.value[deptId] = '' // reset error

  try {
    await api.delete(`/admin/deletedepartment/${deptId}`)

    // remove dept from list
    departments.value = departments.value.filter(d => d.id !== deptId)
  } catch (err) {
    console.error('❌ Delete failed:', err)
    const userMsg = err.response?.status === 400 || err.response?.status === 409
      ? 'Cannot delete: Department has linked categories.'
      : 'Failed to delete department.'

    errorMessageByDept.value[deptId] = userMsg
  }
}

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
  .delete-dept-button {
  background-color: #ff4c4c;
  border-color: #ff4c4c;
  color: white;
}

.delete-dept-button:hover {
  background-color: #c23b3b;
  border-color: #c23b3b;
  color: white;
}

.delete-error {
  color: lightcoral;
  font-size: 0.9rem;
  margin-top: 0.5rem;
  margin-left: 1rem;
}

  </style>
  