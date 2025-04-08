import axios from 'axios';

// make calls through api so users are redirected to login if
// dotnet returns 401
const api = axios.create({
  baseURL: 'http://localhost:5172',
});

// add token to all requests
api.interceptors.request.use(config => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Intercept 401 responses globally
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response && error.response.status === 401) {
      console.warn('🔐 Unauthorized - redirecting to login...');
      localStorage.clear();
      window.location.href = '/';
    }
    return Promise.reject(error);
  }
);

export default api;
