<template>
    <AdminNavbar />
    <div class="overlay"></div>
  
    <div class="profile-page">
      <h1>User Profile</h1>
  
      <div v-if="user" class="profile-card">
        <p><strong>Full Name:</strong> {{ user.fullName }}</p>
        <p><strong>First Name:</strong> {{ user.firstName }}</p>
        <p><strong>Last Name:</strong> {{ user.lastName }}</p>
        <p><strong>Email:</strong> {{ user.email }}</p>
  
        <div class="roles">
          <p><strong>Current Roles:</strong></p>
          <ul>
            <li v-for="role in user.roles" :key="role">{{ role }}</li>
          </ul>
        </div>
  <!-- 🔧 Role Editor -->
<div class="role-editor">
  <p><strong>Edit Roles:</strong></p>

  <div class="predefined-roles">
  <label
    v-for="role in predefinedRoles"
    :key="role"
    :class="{ activeRole: hasRole(role) }"
  >
    <input
      type="checkbox"
      :value="role"
      :checked="hasRole(role)"
      @change="toggleRole(role)"
    />
    {{ role }}
  </label>
</div>


  <div class="custom-role-input">
    <input
      v-model="customRole"
      type="text"
      placeholder="If there is a custom department set its name as the role"
    />
    <button @click="addCustomRole">Add Role</button>
  </div>

  <div class="role-actions">
    <button @click="submitRoleChange" class="save-btn">Save Changes</button>
    <button @click="setUserOnly" class="user-only-btn">Set as User Only</button>
  </div>
</div>

      </div>
  
      <p v-else>Loading profile...</p>
    </div>
  </template>
  
  
  
<script setup>import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import AdminNavbar from '../../components/adminComponents/AdminNavbar.vue';
import { getUserById, changeUserRole } from '../../api/adminApi';

const route = useRoute();
const userId = route.params.userId;

const user = ref(null);
const predefinedRoles = ['Admin', 'HR', 'IT', 'User'];
const selectedRoles = ref([]);
const customRole = ref('');

onMounted(async () => {
  try {
    const response = await getUserById(userId);
    user.value = response.data;
    selectedRoles.value = [...user.value.roles];
  } catch (error) {
    console.error('Failed to fetch user profile:', error);
  }
});

const hasRole = (role) => {
  return selectedRoles.value.some(r => r.toLowerCase() === role.toLowerCase());
};

const toggleRole = (role) => {
  const lower = role.toLowerCase();
  const index = selectedRoles.value.findIndex(r => r.toLowerCase() === lower);

  if (index !== -1) {
    selectedRoles.value.splice(index, 1);
  } else {
    selectedRoles.value.push(role);
  }
};


const setUserOnly = async () => {
  selectedRoles.value = ['User'];

    const response = await changeUserRole(userId, ['User']);
    user.value = response.data;
    alert('User is now assigned only the "User" role.');

};

const addCustomRole = () => {
  const trimmed = customRole.value.trim();
  if (trimmed && !selectedRoles.value.includes(trimmed)) {
    selectedRoles.value.push(trimmed);
  }
  customRole.value = '';
};

const submitRoleChange = async () => {
  try {
    const response = await changeUserRole(userId, selectedRoles.value);
    user.value = response.data;
    alert('Roles updated successfully.');
  } catch (error) {
    console.error('Failed to change roles:', error);
    alert('Failed to update roles.');
  }
};


  </script>
  
  <style scoped>


.predefined-roles label {
  display: inline-block;
  margin-right: 1rem;
  color: #ccc;
}

.custom-role-input {
  margin-top: 1rem;
  display: flex;
  gap: 0.5rem;
}

.custom-role-input input {
  flex: 1;
  padding: 0.5rem;
  border-radius: 4px;
  border: 1px solid #888;
  background-color: #1e1e1e;
  color: white;
}

.custom-role-input button {
  padding: 0.5rem 1rem;
  background-color: #444;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.save-btn {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background-color: #3a8ee6;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.save-btn:hover {
  background-color: #2a6fb4;
}

  .role-editor {
  margin-top: 1.5rem;
}

.role-editor label {
  display: block;
  margin-bottom: 0.5rem;
}

.role-editor select {
  width: 100%;
  min-height: 100px;
  padding: 0.5rem;
  margin-bottom: 1rem;
  border-radius: 6px;
  border: 1px solid #888;
  background-color: #1e1e1e;
  color: #fff;
}

.role-editor button {
  padding: 0.5rem 1rem;
  background-color: #3a8ee6;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.role-editor button:hover {
  background-color: #2a6fb4;
}

  .profile-page {
    padding: 2rem;
    color: #eee;
  }
  
  .profile-card {
    background-color: #2c2c2c;
    padding: 1.5rem;
    border-radius: 10px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.6);
    max-width: 500px;
    margin: 0 auto;
  }
  
  .profile-card p {
    margin: 0.5rem 0;
  }
  
  .roles ul {
    padding-left: 1rem;
  }
  
  .roles li {
    list-style-type: disc;
    color: #ccc;
  }
  </style>
  