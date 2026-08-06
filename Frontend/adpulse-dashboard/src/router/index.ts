import { createRouter, createWebHistory } from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/Login.vue'),
    meta: { title: 'Sign In', public: true }
  },
  {
    path: '/',
    name: 'Dashboard',
    component: () => import('../views/Dashboard.vue'),
    meta: { title: 'Dashboard' }
  },
  {
    path: '/campaigns',
    name: 'Campaigns',
    component: () => import('../views/Campaigns.vue'),
    meta: { title: 'Campaigns' }
  },
  {
    path: '/campaigns/create',
    name: 'CampaignCreate',
    component: () => import('../views/CampaignCreate.vue'),
    meta: { title: 'Create Campaign' }
  },
  {
    path: '/campaigns/:id',
    name: 'CampaignDetail',
    component: () => import('../views/CampaignDetail.vue'),
    meta: { title: 'Campaign Details' }
  },
  {
    path: '/campaigns/:id/edit',
    name: 'CampaignEdit',
    component: () => import('../views/CampaignEdit.vue'),
    meta: { title: 'Edit Campaign' }
  },
  {
    path: '/audiences',
    name: 'Audiences',
    component: () => import('../views/Audiences.vue'),
    meta: { title: 'Audiences & Targeting' }
  },
  {
    path: '/analytics',
    name: 'Analytics',
    component: () => import('../views/Analytics.vue'),
    meta: { title: 'Analytics & Reporting' }
  },
  {
    path: '/events',
    name: 'Events',
    component: () => import('../views/Events.vue'),
    meta: { title: 'Event Ingestion' }
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, _from, next) => {
  document.title = `${to.meta.title || 'Platform'} - AdPulse`;

  const token = localStorage.getItem('adpulse_token');
  const isPublic = to.meta.public === true;

  if (!token && !isPublic) {
    next('/login');
  } else if (token && to.path === '/login') {
    next('/');
  } else {
    next();
  }
});

export default router;
