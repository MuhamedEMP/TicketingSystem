<script setup>
import { uploadToSharePoint } from '../api/sharepointUploader';
import UserNavbar from '../components/UserNavbar.vue';

const handleFileUpload = async (event) => {
  const file = event.target.files[0];
  if (!file) return;

  try {
    const url = await uploadToSharePoint(file);
    console.log("📎 File URL:", url);
  } catch (err) {
    console.error("❌ SharePoint upload failed:", err.message);
  }
};
</script>



<template>
    <UserNavbar></UserNavbar>
  <div class="upload-container">
    <h2>Upload File to SharePoint</h2>
    <input type="file" @change="handleFileUpload" :disabled="isUploading" />

    <p v-if="isUploading">Uploading...</p>
    <p v-if="fileUrl">✅ Uploaded successfully: <a :href="fileUrl" target="_blank">View File</a></p>
    <p v-if="errorMsg" class="error">{{ errorMsg }}</p>
  </div>
</template>

<style scoped>
.upload-container {
  max-width: 600px;
  margin: 2rem auto;
  padding: 1.5rem;
  background-color: #2a2a2a;
  border-radius: 12px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
  color: #eee;
}
.upload-container h2 {
  color: #42b983;
  margin-bottom: 1rem;
}
input[type="file"] {
  background-color: #1f1f1f;
  color: #eee;
  padding: 0.5rem;
  border-radius: 6px;
  border: 1px solid #444;
  width: 100%;
}
.error {
  color: #ff4d4f;
  margin-top: 1rem;
}
a {
  color: #42b983;
  text-decoration: underline;
}
</style>
