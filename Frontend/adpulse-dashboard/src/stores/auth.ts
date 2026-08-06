import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { authApi } from '../api/auth';
import type { User, AuthResponse } from '../types';

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('adpulse_token'));
  const user = ref<User | null>(null);

  const cached = localStorage.getItem('adpulse_user');
  if (cached) {
    try {
      user.value = JSON.parse(cached);
    } catch {
      // ignore JSON parse error
    }
  }

  const isAuthenticated = computed(() => !!token.value);
  const currentTenantId = computed(() => user.value?.tenantId || '');
  const tenantName = computed(() => (user.value?.tenantId ? 'Acme Corporation' : 'Acme Corporation'));
  const userEmail = computed(() => user.value?.email || '');
  const userRole = computed(() => user.value?.role || 'User');

  function setAuth(authData: AuthResponse) {
    token.value = authData.token;
    user.value = {
      userId: authData.userId,
      email: authData.email,
      role: authData.role,
      tenantId: authData.tenantId,
      firstName: authData.firstName,
      lastName: authData.lastName
    };
    localStorage.setItem('adpulse_token', authData.token);
    localStorage.setItem('adpulse_user', JSON.stringify(user.value));
  }

  async function login(email: string, password: string): Promise<boolean> {
    try {
      const data = await authApi.login(email, password);
      setAuth(data);
      return true;
    } catch (error) {
      console.error('Login failed:', error);
      return false;
    }
  }

  async function register(payload: {
    tenantName: string;
    companyName: string;
    email: string;
    password: string;
    firstName: string;
    lastName: string;
  }): Promise<boolean> {
    try {
      const data = await authApi.register(payload);
      setAuth(data);
      return true;
    } catch (error) {
      console.error('Registration failed:', error);
      return false;
    }
  }

  function logout() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('adpulse_token');
    localStorage.removeItem('adpulse_user');
  }

  return {
    token,
    user,
    isAuthenticated,
    currentTenantId,
    tenantName,
    userEmail,
    userRole,
    login,
    register,
    logout
  };
});
