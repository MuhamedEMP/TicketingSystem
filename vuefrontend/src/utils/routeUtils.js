import { computed } from 'vue';
import { useRoute } from 'vue-router';

export function useRouteFlags() {
  const route = useRoute();

  const homePage = computed(() => route.path === '/home');
  const notHome = computed(() => route.path !== '/home');
  const notMyTickets = computed(() => route.path !== '/user/mytickets');

  return { homePage, notHome, notMyTickets };
}
