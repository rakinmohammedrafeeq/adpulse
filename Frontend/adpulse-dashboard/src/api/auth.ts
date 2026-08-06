import client from './client';
import type { AuthResponse, User } from '../types';

export const authApi = {
  login: async (email: string, password: string): Promise<AuthResponse> => {
    const res = await client.post<AuthResponse>('/api/auth/login', { email, password });
    return res.data;
  },

  register: async (payload: {
    tenantName: string;
    companyName: string;
    email: string;
    password: string;
    firstName: string;
    lastName: string;
  }): Promise<AuthResponse> => {
    const res = await client.post<AuthResponse>('/api/auth/register', payload);
    return res.data;
  },

  getCurrentUser: async (): Promise<User> => {
    const res = await client.get<User>('/api/auth/me');
    return res.data;
  }
};
