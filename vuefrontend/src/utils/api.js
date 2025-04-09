import axios from 'axios';
import router from '../router';
import { refreshUserRoles } from './userUtils';

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

// // Intercept 401 responses globally
// api.interceptors.response.use(
//   response => response,
//   async error => {
//     const status = error.response?.status;

//     if (status === 401) {
//       localStorage.clear();
//       router.push('/unauthorized');
//     } else if (status === 403) {  // gets users roles from the db in case they were updated
//       router.push("/forbidden");
//       console.log("INTERCEPTED 403"); 
//       await refreshUserRoles();
//       console.log("AFTER AWAIT"); 

//     } else if (status === 500) {
//       router.push('/internal');
//     }

//     return Promise.reject(error);
//   }
// );


export default api;
