<template>
  <div class="login-page">
    <div class="login-box">
      <img class="ms-logo" src="../assets/microsoft-logo.png" alt="Microsoft Logo" />
      <h1 class="login-heading">Login to Ticketing System</h1>
      <button class="ms-login-button" @click="login">
        <span class="ms-button-text">Sign in with Microsoft</span>
      </button>
    </div>
  </div>
</template>

<script setup>
import { getCurrentInstance } from "vue";
import { loginRequest } from "../authConfig";

const { appContext } = getCurrentInstance();
const msal = appContext.config.globalProperties.$msal;

const login = () => {
  msal.loginRedirect(loginRequest);
};
</script>

<style scoped>
.login-page {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  font-family: 'Poppins', sans-serif;
  background: none; /* No background color */
}

.login-box {
  background-color: white; /* Visible box */
  padding: 40px;
  border-radius: 12px;
  text-align: center;
  color: #333;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); /* subtle shadow */
  display: flex;
  flex-direction: column;
  align-items: center;
}

.ms-logo {
  width: 48px;
  height: 48px;
  margin-bottom: 20px;
}

.login-heading {
  font-size: 1.8rem;
  margin-bottom: 30px;
  color: #111;
}

.ms-login-button {
  background-color: #0078D4; /* Microsoft blue */
  color: white;
  font-weight: 600;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  cursor: pointer;
  font-family: 'Poppins', sans-serif;
  display: flex;
  align-items: center;
  transition: all 0.3s ease;
}

.ms-login-button:hover {
  background-color: #005a9e;
  transform: translateY(-2px);
}

.ms-button-text {
  margin-left: 8px;
}
</style>
