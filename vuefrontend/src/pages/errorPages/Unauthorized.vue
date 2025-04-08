<template>
    <div class="error-page">
      <h1>401 - Unauthorized</h1>
      <p>You must log in to access this page.</p>
      <router-link to="/">Go to Login</router-link>
    </div>
  </template>
  
  <style scoped>
  .error-page {
    text-align: center;
    padding: 2rem;
  }
  </style>
  