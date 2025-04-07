import { createRouter, createWebHistory } from 'vue-router'; // not hash mode
import Login from './pages/Login.vue';
import Home from './pages/Home.vue';
import UserMyTickets from './pages/UserMyTickets.vue';
import NewTicket from './pages/NewTicket.vue';


const routes = [
  { path: '/', component: Login },
  { path: '/home', component: Home },
  { path: '/user/mytickets', component: UserMyTickets },
  { path: "/user/newticket", component: NewTicket },
];

export const router = createRouter({
  history: createWebHistory(), // <- ✅ must be history mode
  routes,
});
