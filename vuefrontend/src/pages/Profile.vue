<template>
  <UserNavbar />
  <div class="overlay"></div>

  <div class="profile-page">
    <h1>My Profile</h1>

    <div v-if="user" class="profile-card">
      <p><strong>Full Name:</strong> {{ user.fullName }}</p>
      <p><strong>Email:</strong> {{ user.email }}</p>

      <!-- 🎯 Access Summary -->
      <div class="access-summary">
        <p><strong>Current Access:</strong></p>
        <ul>
          <li v-if="user.isAdmin">🔑 Admin Access</li>
          <li v-if="user.accessibleDepartmentDtos?.length">
            🏢 Departments:
            <ul>
              <li v-for="dept in user.accessibleDepartmentDtos" :key="dept.id">
                {{ dept.name }}
              </li>
            </ul>
          </li>
          <li v-if="!user.isAdmin && !user.accessibleDepartmentDtos?.length">
            👤 Regular User
          </li>
        </ul>
      </div>
    </div>

    <p v-else>Loading profile...</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import UserNavbar from '../components/UserNavbar.vue';
import { getUserProfile } from '../api/userApi';

const user = ref(null);

onMounted(async () => {
  try {
    user.value = await getUserProfile();
  } catch (error) {
    console.error('❌ Failed to fetch user profile:', error);
  }
});
</script>

<style scoped>
.profile-page {
  padding: 2rem;
  color: #eee;
}

.profile-card {
  background-color: white;
  padding: 1.5rem;
  color: black;
  border-radius: 12px;
  max-width: 600px;
  margin: 0 auto;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.4);
}

.profile-card p {
  margin: 0.5rem 0;
}

.access-summary {
  margin-top: 1rem;
  color: black;
  background: white;
  padding: 1rem;
  border-radius: 8px;
}

.access-summary ul {
  margin-left: 1.5rem;
  color: black;
}

.access-summary li {
  margin-bottom: 0.25rem;
}
</style>
