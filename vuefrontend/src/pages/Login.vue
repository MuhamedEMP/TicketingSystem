<template>
    <div>
      <h1>Ticketing System (Vue)</h1>
      <button @click="login">Login with Microsoft</button>
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
  