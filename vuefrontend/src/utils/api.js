import axios from 'axios';
import router from '../router';
import { refreshUserRoles } from './userUtils';

// make calls through api 
const api = axios.create({
  baseURL: 'http://localhost:5172',
});

// add token to all requests and refresh roles in local storage on each request
// MAY SLOW DOWN THE APP ?
api.interceptors.request.use(config => {
  return refreshUserRoles().then(() => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });
});



export default api;
