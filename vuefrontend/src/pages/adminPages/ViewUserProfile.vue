<template>
  <AdminNavbar />
  <div class="overlay"></div>

  <div class="profile-page">
    <h1>User Profile</h1>

    <div v-if="user" class="profile-card">
      <p><strong>Full Name:</strong> {{ user.fullName }}</p>
      <p><strong>Email:</strong> {{ user.email }}</p>

      <!-- 🧾 Display Current Access -->
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

      <!-- 🛠 Edit Access -->
      <div class="access-editor">
        <h3>Edit Access</h3>

        <div class="toggle-section">
          <label>
            <input type="checkbox" v-model="isAdmin" />
            Grant Admin Access
          </label>
        </div>

        <div class="dept-checkboxes">
          <p><strong>Accessible Departments:</strong></p>
          <label
            v-for="dept in departments"
            :key="dept.id"
            class="checkbox"
          >
            <input
              type="checkbox"
              :value="dept.id"
              v-model="selectedDepartmentIds"
              :disabled="false"
            />
            {{ dept.name }}
          </label>
        </div>

        <div class="action-buttons">
          <button @click="saveChanges" class="save-btn">💾 Save</button>
          <button @click="setAsUserOnly" class="user-only-btn">
            👤 Set as Regular User
          </button>
        </div>
      </div>
    </div>

    <p v-else>Loading profile...</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import AdminNavbar from '../../components/adminComponents/AdminNavbar.vue';
import { getUserById, changeUserRole } from '../../api/adminApi';
import { getAllDepartments } from '../../api/departmentApi';

const route = useRoute();
const userId = route.params.userId;

const user = ref(null);
const departments = ref([]);
const isAdmin = ref(false);
const selectedDepartmentIds = ref([]);

const fetchProfile = async () => {
  try {
    const [userRes, deptRes] = await Promise.all([
      getUserById(userId),
      getAllDepartments()
    ]);

    user.value = userRes.data;
    departments.value = deptRes;

    isAdmin.value = user.value.isAdmin;

    // Sync selected departments
    selectedDepartmentIds.value = user.value.accessibleDepartmentDtos?.map(
      d => {
        const match = departments.value.find(dep => dep.name === d.name);
        return match?.id;
      }
    ).filter(Boolean) || [];

  } catch (err) {
    console.error('❌ Failed to load user or departments:', err);
  }
};

onMounted(fetchProfile);

const saveChanges = async () => {
  try {
    const payload = {
      isAdmin: isAdmin.value,
      DepartmentIds: selectedDepartmentIds.value
    };

    const res = await changeUserRole(userId, payload);
    user.value = res.data;
    await fetchProfile(); // Refresh state after update
    alert("✅ Access updated successfully.");
  } catch (err) {
    console.error("❌ Failed to update role:", err);
    alert("Something went wrong.");
  }
};

const setAsUserOnly = async () => {
  isAdmin.value = false;
  selectedDepartmentIds.value = [];
  await saveChanges();
};
</script>

<style scoped>
.profile-page {
  padding: 2rem;
  color: black;
}

.profile-card {
  background-color: white;
  padding: 1.5rem;
  border-radius: 12px;
  max-width: 600px;
  margin: 0 auto;
  box-shadow: 0 0 10px rgba(0,0,0,0.4);
}

.access-summary {
  margin-top: 1rem;
  background: white;
  padding: 1rem;
  border-radius: 8px;
}

.access-summary ul {
  margin-left: 1.5rem;
  color: black;
}

.access-editor {
  margin-top: 2rem;
  padding: 1rem;
  background-color: white;
  border-radius: 10px;
}

.toggle-section {
  margin-bottom: 1rem;
}

.dept-checkboxes {
  margin: 1rem 0;
}

.checkbox {
  display: inline-block;
  margin: 0.25rem 1rem 0.25rem 0;
  color: black;
}

.action-buttons {
  margin-top: 1rem;
}

.save-btn,
.user-only-btn {
  padding: 0.6rem 1.2rem;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  margin-right: 1rem;
}

.save-btn {
  background-color: #ca0176;
  color: white;
}

.user-only-btn {
  background-color: #ca0176;
  color: white;
}
</style>
