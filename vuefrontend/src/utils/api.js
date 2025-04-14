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
    if (error.response) {
      const status = error.response.status;

      // prevent infinite loop
      const originalRequest = error.config;
      if (originalRequest._retry) {
        router.push('/unauthorized');
        return Promise.reject(error);
      }

      if (status === 403) {
        try {
          originalRequest._retry = true;

          await refreshUserRoles();
          return api(originalRequest); // retry with refreshed roles
        } catch (refreshError) {
          router.push('/unauthorized');
        }
      }

      if (status === 401) {
        localStorage.clear();
        router.push('/');
      }
    }

    return Promise.reject(error);
  }
);


export default api;
