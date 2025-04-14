<template>
    <div>
      <AdminNavbar />
      <div class="overlay"></div>
  
      <div class="add-category-page">
        <h1>Add New Category for Department: {{ deptName || deptId }}</h1>
  
        <form @submit.prevent="submitCategory">
          <div class="add-category-block">
            <label class="add-category-label">Category Name</label>
            <input
              v-model="name"
              class="add-category-input"
              placeholder="Enter name"
              required
            />
  
            <label class="add-category-label">Description (optional)</label>
            <input
              v-model="description"
              class="add-category-input"
              placeholder="Enter description"
            />
  
            <button type="submit" class="add-category-button add-category-small-button">
              Add Category
            </button>
          </div>
        </form>
  
        <p v-if="successMessage" class="success-msg">{{ successMessage }}</p>
        <p v-if="errorMessage" class="error-msg">{{ errorMessage }}</p>
      </div>
    </div>
  </template>
  
  
  <script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { addNewCategory } from '../../api/adminApi'
import { getDepartmentById } from '../../api/departmentApi'
import AdminNavbar from '../../components/adminComponents/AdminNavbar.vue'

const route = useRoute()
const deptId = route.params.deptId

const name = ref('')
const description = ref('')
const deptName = ref('')
const successMessage = ref('')
const errorMessage = ref('')

const submitCategory = async () => {
  try {
    await addNewCategory({
      name: name.value,
      description: description.value || null,
      departmentId: parseInt(deptId)
    })

    successMessage.value = '✅ Category added successfully.'
    errorMessage.value = ''
    name.value = ''
    description.value = ''
  } catch (err) {
    errorMessage.value = '❌ Failed to add category.'
    successMessage.value = ''
    console.error(err)
  }
}

onMounted(async () => {
  try {
    const department = await getDepartmentById(deptId)
    deptName.value = department.name
  } catch (err) {
    console.error('Failed to load department name:', err)
  }
})
</script>

  
  <style scoped>
  .add-category-page {
    padding: 2rem;
    color: #eee;
  }
  
  .add-category-block {
    background-color: #2a2a2a;
    padding: 1rem;
    border-radius: 10px;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-top: 1rem;
    max-width: 500px;
  }
  
  .add-category-input {
    padding: 0.6rem;
    background-color: #1e1e1e;
    color: #eee;
    border: 1px solid #555;
    border-radius: 5px;
    outline: none;
  }
  
  .add-category-input:focus {
    border-color: #42b983;
  }
  
  .add-category-label {
    font-weight: bold;
    color: #ccc;
  }
  
  .add-category-button {
    background-color: whitesmoke;
    border: 2px solid #232020;
    border-radius: 25px;
    font-size: 16px;
    font-weight: bold;
    color: #232020;
    cursor: pointer;
    transition: all 0.3s ease;
    text-align: center;
  }
  
  .add-category-small-button {
    padding: 8px 16px;
    font-size: 14px;
    border-radius: 20px;
  }
  
  .add-category-button:hover {
    background-color: #232020;
    color: whitesmoke;
  }
  
  .success-msg {
    margin-top: 1rem;
    color: lightgreen;
  }
  
  .error-msg {
    margin-top: 1rem;
    color: lightcoral;
  }
  </style>
  
  