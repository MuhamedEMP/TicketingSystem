<template>
  <AdminNavbar />
  <div class="admin-users">
    <h1>All Registered Users</h1>

    <!-- 🔍 Multi-field search -->

    <div class="checkbox-new">
      <div class="checkbox-wrapper-4">
        <input class="inp-cbx" id="admin" type="checkbox" v-model="query.isAdmin" :true-value="true" :false-value="null" />
        <label class="cbx" for="admin"><span>
          <svg width="12px" height="10px">
            <use xlink:href="#check-4"></use>
          </svg></span><span>Admin</span>
        </label>
    </div>


</div>

    <div class="search-form">
      <input v-model="query.firstName" placeholder="First Name" />
      <input v-model="query.lastName" placeholder="Last Name" />
      <input v-model="query.email" placeholder="Email" />

        <div class="checkbox-row">
    </div>
      <button @click="searchUsers">Search</button>
      <button @click="resetFilters" v-if="isFiltering" class="reset-button">Reset</button>
    </div>

    

    <table>
      <thead>
        <tr>
          <th>Full Name</th>
          <th>Email</th>
          <th>Role Type</th>
          <th>Accessible Departments</th>
          <th>Actions</th> 
        </tr>
      </thead>

      <tbody>
        <tr v-for="user in users" :key="user.userId">
          <td>{{ user.fullName }}</td>
          <td>{{ user.email }}</td>
          <td>
            {{ user.roleType || (user.isAdmin ? "Admin" : user.accessibleDepartmentDtos?.length ? "Department User" : "Regular User") }}
          </td>
          <td>
            <ul v-if="user.accessibleDepartmentDtos?.length">
              <li v-for="dept in user.accessibleDepartmentDtos" :key="dept.id">{{ dept.name }}</li>
            </ul>
            <span v-else>-</span>
          </td>
          <td>
            <router-link :to="`/admin/user/${user.userId}`">View Profile</router-link>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

  
<script setup>
import AdminNavbar from '../../components/adminComponents/AdminNavbar.vue';
import { ref, onMounted, computed } from 'vue';
import { getAllUsers, queryUsers } from '../../api/adminApi';

const users = ref([]);
const query = ref({
  firstName: '',
  lastName: '',
  fullName: '',
  email: '',
  isAdmin: null,
  hasDepartments: null
});


const isFiltering = computed(() => {
  return Object.entries(query.value).some(([key, val]) => {
    if (typeof val === 'string') return val.trim() !== '';
    if (typeof val === 'boolean') return val === true;
    return false;
  });
});


onMounted(async () => {
  try {
    const response = await getAllUsers();
    users.value = response.data;
  } catch (err) {
    console.error("Failed to fetch users:", err);
  }
});

const searchUsers = async () => {
  try {
    const response = await queryUsers(query.value);
    users.value = response.data;
  } catch (err) {
    console.error("Search failed:", err);
  }
};

const resetFilters = async () => {
  query.value = {
    firstName: '',
    lastName: '',
    fullName: '',
    email: ''
  };

  try {
    const response = await getAllUsers();
    users.value = response.data;
  } catch (err) {
    console.error("Failed to reload users:", err);
  }
};
</script>

  

<style scoped>
.checkbox-row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  margin-top: 0.5rem;
  align-items: center;
}

.checkbox-item {
  display: flex;
  align-items: center;
  gap: 0; /* tighter spacing between checkbox and label text */
  font-size: 0.95rem;
  padding: 0.2rem 0.5rem; /* less horizontal padding */
  background-color: #2a2a2a;
  border: 1px solid #444;
  border-radius: 6px;
  color: #eee;
  cursor: pointer;
  transition: background-color 0.2s;
}

.checkbox-item:hover {
  background-color: #3a3a3a;
}

.checkbox-item input {
  accent-color: #3a8ee6;
  cursor: pointer;
  margin: 0; /* remove default margin around checkbox */
}


.checkbox-group {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
  margin-top: 0.5rem;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.95rem;
  color: #ccc;
}


.admin-users {
  padding: 2rem;
  color: #eee;
  max-width: 1200px;
  margin: auto;
}

h1 {
  font-size: 2rem;
  margin-bottom: 1.5rem;
}

.search-form {
  margin-bottom: 1.5rem;
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
  border-radius: 8px;
}

.search-form input {
  padding: 0.6rem;
  min-width: 180px;
  border-radius: 6px;
  border: 1px solid #444;
  background-color: white;
  color: black;
}

.search-form button {
  padding: 0.6rem 1.2rem;
  background-color: #ca0176;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.search-form button:hover {
  background-color: white;
  color: #ca0176;
}

.reset-button {
  background-color: #555;
  color: #fff;
  border: none;
}

.reset-button:hover {
  background-color: #822;
}

table {
  width: 100%;
  border-collapse: collapse;
  background-color: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 0 10px rgba(0,0,0,0.3);
}

thead {
  background-color: #2a2a2a;
}

th, td {
  padding: 0.85rem;
  border: 1px solid #333;
  text-align: left;
  color: black;
}

th {
  color: #eee;
  font-weight: 600;
}

td ul {
  padding-left: 1rem;
  margin: 0;
}

td li {
  list-style-type: disc;
  color: black;
}

a {
  color: #ca0176;
  text-decoration: none;
  font-weight: 500;
}

a:hover {
  color: black;
}


</style>
