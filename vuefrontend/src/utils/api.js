import axios from 'axios';
import router from '../router';
import { refreshUserRoles } from './userUtils';

const api = axios.create({
  baseURL: 'http://localhost:5172',
});

// Add token to every request
api.interceptors.request.use(async (config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle 403 and 401 globally
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response) {
      const status = error.response.status;

      // 🔐 Handle 401 (unauthenticated)
      if (status === 401) {
        localStorage.clear();
        router.push('/');
        return Promise.reject(error);
      }

      // 🔒 Handle 403 (forbidden)
      if (status === 403) {
        if (!originalRequest._retry) {
          originalRequest._retry = true;
          try {
            await refreshUserRoles();
            return api(originalRequest);
          } catch (refreshErr) {
            console.error('🔁 Failed to refresh roles:', refreshErr);
          }
        }

        // Retry failed or didn't help → forbidden
        router.push('/forbidden');
        return Promise.reject(error);
      }
    }

    return Promise.reject(error);
  }
);



export default api;
