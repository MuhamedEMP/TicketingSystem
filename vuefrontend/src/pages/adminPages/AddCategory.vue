<template>
  <div>
    <AdminNavbar />
    <div class="overlay"></div>

    <div class="add-category-page">
      <div class="add-category-wrapper">
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
  display: flex;
  flex-direction: column;
  align-items: center;
}

.add-category-wrapper {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
}

form {
  width: 100%;
  display: flex;
  justify-content: center;
}

.add-category-block {
  background-color: white;
  padding: 1.5rem;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-top: 1rem;
  width: 100%;
  max-width: 400px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  align-items: stretch;
}

.add-category-label {
  font-weight: bold;
  color: black;
  text-align: left;
}

.add-category-input {
  padding: 0.6rem;
  background-color: white;
  color: black;
  border: 1px solid #555;
  border-radius: 5px;
  outline: none;
  width: 100%;
  box-sizing: border-box;
}

.add-category-input:focus {
  border-color: #ca0176;
}

.add-category-button {
  background-color: #ca0176;
  border: 2px solid #ca0176;
  border-radius: 25px;
  font-size: 16px;
  font-weight: bold;
  color: white;
  cursor: pointer;
  transition: all 0.3s ease;
  text-align: center;
  width: 100%;
  padding: 10px;
}

.add-category-small-button {
  font-size: 14px;
}

.add-category-button:hover {
  background-color: white;
  color: #ca0176;
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
