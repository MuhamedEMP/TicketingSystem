import { createRouter, createWebHistory } from 'vue-router'; // not hash mode
import Login from './pages/Login.vue';
import Home from './pages/Home.vue';
import UserMyTickets from './pages/UserMyTickets.vue';
import NewTicket from './pages/NewTicket.vue';
import CategoriesByDepartment from './pages/CategoriesByDepartment.vue';
import Profile from './pages/Profile.vue';
import Unauthorized from './pages/errorPages/Unauthorized.vue';
import Forbidden from './pages/errorPages/Forbidden.vue';
import InternalServerError from './pages/errorPages/InternalServerError.vue';
import AddDepartment from './pages/adminPages/AddDepartment.vue';
import AddCategory from './pages/adminPages/AddCategory.vue';
import ViewDepartments from './pages/adminPages/ViewDepartments.vue';
import MyDepartments from './pages/DeptUserPages/MyDepartments.vue';
import DepartmentTickets from './pages/DeptUserPages/DepartmentTickets.vue';
import DeptViewTicket from './pages/DeptUserPages/DeptViewTicket.vue';

const routes = [
  { path: '/unauthorized', component: Unauthorized },
  { path: '/forbidden', component: Forbidden },
  { path: '/internal', component: InternalServerError },
  { path: '/', component: Login },
  { path: '/home', component: Home, // avaliable to all users, will conditionally display data
    meta: {
      requiresAuth: true
    }
   },
  { path: '/user/mytickets', component: UserMyTickets,
    meta: {
      requiresAuth: true,
      roles: ['user'] // if not 403
    }
   },
  { path: '/profile', component: Profile },
  {
    path: '/newticket/:departmentId/:categoryId',
    name: 'NewTicket',
    component: () => import('./pages/NewTicket.vue'),
  },
  {
    path: '/user/tickets/:id',
    name: 'ViewTicket',
    component: () => import('./pages/ViewTicket.vue')
  }, 
  {
    path: '/admin',
    component: () => import('./pages/adminPages/AdminDashboard.vue'),
    meta: {
      requiresAuth: true,
      roles: ['Admin'] // if not 403
    }
  },
  {
    path: '/admin/users',
    component: () => import('./pages/adminPages/AdminViewUsers.vue'),
    meta: {
      requiresAuth: true,
      roles: ['Admin'] // if not 403
    }
  },
  {
    path: '/admin/user/:userId',
    component: () => import('./pages/adminPages/ViewUserProfile.vue'),
    meta: {
      requiresAuth: true,
      roles: ['Admin'] // if not 403
    }
  },
  { path: '/department/:deptId/categories', component: CategoriesByDepartment,
    meta: {
      requiresAuth: true,
    }
   },
   { path: '/admin/adddepartment', component: AddDepartment,
    meta: {
      requiresAuth: true,
      roles: ['Admin'] // if not 403
    }
   },
   { path: '/admin/departments', component: ViewDepartments,
    meta: {
      requiresAuth: true,
      roles: ['Admin'] // if not 403
    }
   },
   {
    path: '/department/:deptId/categories/add',
    component: AddCategory,
    meta: { requiresAuth: true, roles: ['admin'] }
  }
  ,
  {
    path: '/mydepartments',
    component: MyDepartments,
    meta: { requiresAuth: true }
  },
  {
    path: '/sharedtickets',
    component: DepartmentTickets,
    meta: { requiresAuth: true }
  },
  {
    path: '/sharedtickets/:id',
    component: DeptViewTicket,
    meta: { requiresAuth: true }
  }
  
];


export const router = createRouter({
  history: createWebHistory(), 
  routes,
});


export default router;