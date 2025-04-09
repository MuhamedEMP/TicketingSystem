<template>
    <AdminNavbar />
    <div class="admin-users">
      <h1>All Registered Users</h1>
  
      <!-- 🔍 Multi-field search -->
      <div class="search-form">
        <input v-model="query.firstName" placeholder="First Name" />
        <input v-model="query.lastName" placeholder="Last Name" />
        <input v-model="query.fullName" placeholder="Full Name" />
        <input v-model="query.email" placeholder="Email" />
        <input v-model="query.role" placeholder="Role" />

        <button @click="searchUsers">Search</button>
        <button @click="resetFilters" v-if="isFiltering" class="reset-button">Reset</button>
      </div>
  
      <table>
        <thead>
            <tr>
                <th>Full Name</th>
                <th>Email</th>
                <th>Roles</th>
                <th>Actions</th> 
            </tr>
        </thead>

        <tbody>
            <tr v-for="user in users" :key="user.email">
                <td>{{ user.fullName }}</td>
                <td>{{ user.email }}</td>
                <td>{{ user.roles.join(', ') }}</td>
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
  import { ref, onMounted } from 'vue';
  import { getAllUsers, queryUsers } from '../../api/adminApi';
  import { computed } from 'vue';
 
  const users = ref([]);
  const query = ref({
    firstName: '',
    lastName: '',
    fullName: '',
    email: '',
    role: ''
  });

  const isFiltering = computed(() => {
  return Object.values(query.value).some(val => val.trim() !== '');
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
    email: '',
    role: ''
  };


  // Reload full user list
  try {
    const response = await getAllUsers();
    users.value = response.data;
  } catch (err) {
    console.error("Failed to reload users:", err);
  }
};
  </script>
  

<style scoped>
.admin-users {
  padding: 2rem;
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead {
  background-color: #f3f3f3;
}

th, td {
  padding: 0.75rem;
  border: 1px solid #ccc;
  text-align: left;
}

.search-bar {
  margin-bottom: 1rem;
}

.search-bar input {
  padding: 0.5rem;
  width: 300px;
  margin-right: 0.5rem;
}

.search-bar button {
  padding: 0.5rem 1rem;
}
.search-form {
  margin-bottom: 1rem;
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.search-form input {
  padding: 0.5rem;
  min-width: 150px;
}

.search-form button {
  padding: 0.5rem 1rem;
}

.reset-button {
  background-color: #eee;
  color: #ba0a0a;
  border: 1px solid #ccc;
}

</style>
