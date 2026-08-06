import axios from 'axios';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const client = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 10000
});

// Request interceptor to attach JWT Bearer token
client.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('adpulse_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor to handle auth expiration
client.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      // Clear token and redirect to login if not already on login page
      if (!window.location.pathname.includes('/login')) {
        localStorage.removeItem('adpulse_token');
        localStorage.removeItem('adpulse_user');
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export default client;
