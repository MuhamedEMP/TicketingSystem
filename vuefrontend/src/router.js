import { createRouter, createWebHistory } from 'vue-router'; // not hash mode
import Login from './pages/Login.vue';
import Home from './pages/Home.vue';
import UserMyTickets from './pages/UserMyTickets.vue';
import NewTicket from './pages/NewTicket.vue';
import CategoriesByDepartment from './pages/CategoriesByDepartment.vue';
import Profile from './pages/Profile.vue';
import Unauthorized from './pages/errorPages/Unauthorized.vue';
import Forbidden from './pages/errorPages/Forbidden.vue';
import AdminDashboard from './pages/AdminDashboard.vue';


const routes = [
  { path: '/unauthorized', component: Unauthorized },
  { path: '/forbidden', component: Forbidden },
  { path: '/', component: Login },
  { path: '/home', component: Home },
  { path: '/user/mytickets', component: UserMyTickets },
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
    component: () => import('./pages/AdminDashboard.vue'),
    meta: {
      requiresAuth: true,
      roles: ['Admin'] // if not 403
    }
  },
  { path: '/department/:deptId/categories', component: CategoriesByDepartment },
];


export const router = createRouter({
  history: createWebHistory(), 
  routes,
});


router.beforeEach((to, from, next) => {
  const requiresAuth = to.meta.requiresAuth;
  const requiredRoles = to.meta.roles || [];

  const token = localStorage.getItem('accessToken');
  const storedRoles = JSON.parse(localStorage.getItem('roles') || '[]');
  const userRoles = storedRoles.map((r) => r.toLowerCase());

  if (requiresAuth && !token) {
    return next('/unauthorized');
  }

  if (requiresAuth && requiredRoles.length > 0) {
    const hasAccess = requiredRoles.some((role) =>
      userRoles.includes(role.toLowerCase())
    );

    if (!hasAccess) {
      return next('/forbidden');
    }
  }

  next();
});

