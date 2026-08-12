<template>
  <div id="app">
    <!-- Authenticated Layout -->
    <div v-if="authStore.isAuthenticated" class="app-layout">
      <header class="app-header">
        <div class="header-left">
          <router-link to="/" class="brand-link">
            <AdPulseLogo size="medium" :tagline="true" />
          </router-link>

          <div class="tenant-badge">
            <span class="live-pulse"></span>
            <span class="tenant-name">{{ authStore.tenantName || 'Acme Corporation' }}</span>
          </div>

          <nav class="nav-links">
            <router-link to="/" class="nav-item">
              <el-icon class="nav-icon"><Odometer /></el-icon>
              <span>Dashboard</span>
            </router-link>
            <router-link to="/campaigns" class="nav-item">
              <el-icon class="nav-icon"><TrendCharts /></el-icon>
              <span>Campaigns</span>
            </router-link>
            <router-link to="/audiences" class="nav-item">
              <el-icon class="nav-icon"><UserFilled /></el-icon>
              <span>Audiences</span>
            </router-link>
            <router-link to="/analytics" class="nav-item">
              <el-icon class="nav-icon"><DataLine /></el-icon>
              <span>Analytics</span>
            </router-link>
            <router-link to="/events" class="nav-item">
              <el-icon class="nav-icon"><Lightning /></el-icon>
              <span>Live Feed</span>
            </router-link>
          </nav>
        </div>

        <div class="header-right">
          <div class="system-status-indicator">
            <span class="status-dot"></span>
            <span class="status-label">Attribution Online</span>
          </div>

          <div class="user-profile">
            <div class="user-avatar">
              {{ (authStore.userEmail || 'A')[0].toUpperCase() }}
            </div>
            <div class="user-meta">
              <span class="user-email">{{ authStore.userEmail || 'admin@acme.com' }}</span>
              <span class="user-role">{{ authStore.userRole || 'Admin' }}</span>
            </div>
          </div>

          <el-button
            class="logout-btn"
            size="small"
            icon="SwitchButton"
            @click="handleLogout"
          >
            Logout
          </el-button>
        </div>
      </header>

      <main class="app-main">
        <router-view />
      </main>
    </div>

    <!-- Unauthenticated (Login) Layout -->
    <div v-else class="auth-layout">
      <router-view />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useAuthStore } from './stores/auth';
import AdPulseLogo from './components/AdPulseLogo.vue';
import {
  Odometer,
  TrendCharts,
  UserFilled,
  DataLine,
  Lightning
} from '@element-plus/icons-vue';

const router = useRouter();
const authStore = useAuthStore();

function handleLogout() {
  authStore.logout();
  router.push('/login');
}
</script>

<style>
* {
  box-sizing: border-box;
}

body {
  margin: 0;
  padding: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Inter', 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
  background-color: #0b0f19;
  color: #f1f5f9;
  -webkit-font-smoothing: antialiased;
}

#app {
  min-height: 100vh;
}

.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-header {
  background: rgba(15, 23, 42, 0.85);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  height: 64px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 28px;
  position: sticky;
  top: 0;
  z-index: 100;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.header-left {
  display: flex;
  align-items: center;
  gap: 24px;
}

.brand-link {
  text-decoration: none;
}

.tenant-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  background: rgba(30, 41, 59, 0.7);
  border: 1px solid rgba(255, 255, 255, 0.1);
  padding: 5px 12px;
  border-radius: 24px;
  font-size: 12px;
  color: #cbd5e1;
}

.live-pulse {
  width: 7px;
  height: 7px;
  background-color: #10b981;
  border-radius: 50%;
  box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
  animation: pulse-ring 2s infinite cubic-bezier(0.455, 0.03, 0.515, 0.955);
}

@keyframes pulse-ring {
  0% {
    box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
  }
  70% {
    box-shadow: 0 0 0 6px rgba(16, 185, 129, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(16, 185, 129, 0);
  }
}

.tenant-name {
  font-weight: 600;
  color: #e2e8f0;
}

.nav-links {
  display: flex;
  gap: 6px;
}

.nav-item {
  color: #94a3b8;
  text-decoration: none;
  padding: 7px 14px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 6px;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

.nav-item:hover {
  color: #f8fafc;
  background: rgba(255, 255, 255, 0.06);
}

.nav-item.router-link-active {
  color: #38bdf8;
  background: rgba(56, 189, 248, 0.12);
  border: 1px solid rgba(56, 189, 248, 0.25);
  font-weight: 600;
}

.nav-icon {
  font-size: 15px;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

.system-status-indicator {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: #34d399;
  background: rgba(52, 211, 153, 0.1);
  padding: 4px 10px;
  border-radius: 20px;
  border: 1px solid rgba(52, 211, 153, 0.2);
}

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #34d399;
}

.user-profile {
  display: flex;
  align-items: center;
  gap: 10px;
}

.user-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, #06b6d4, #3b82f6);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 13px;
  box-shadow: 0 2px 8px rgba(6, 182, 212, 0.3);
}

.user-meta {
  display: flex;
  flex-direction: column;
  line-height: 1.2;
}

.user-email {
  font-size: 12px;
  font-weight: 500;
  color: #e2e8f0;
}

.user-role {
  font-size: 10px;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.logout-btn {
  background: rgba(239, 68, 68, 0.1) !important;
  border-color: rgba(239, 68, 68, 0.25) !important;
  color: #f87171 !important;
  border-radius: 6px;
  transition: all 0.2s ease;
}

.logout-btn:hover {
  background: rgba(239, 68, 68, 0.2) !important;
  color: #ef4444 !important;
}

.app-main {
  flex: 1;
  background: #080c14;
  min-height: calc(100vh - 64px);
}

/* Global Dark Theme Overrides for Element Plus */
.el-card {
  background: rgba(15, 23, 42, 0.85) !important;
  border: 1px solid rgba(255, 255, 255, 0.08) !important;
  color: #f8fafc !important;
  border-radius: 16px !important;
  backdrop-filter: blur(8px) !important;
}

.el-card__header {
  border-bottom: 1px solid rgba(255, 255, 255, 0.08) !important;
  padding: 16px 20px !important;
  color: #f8fafc !important;
  font-weight: 700 !important;
}

.el-table {
  background-color: transparent !important;
  color: #cbd5e1 !important;
  --el-table-border-color: rgba(255, 255, 255, 0.06) !important;
  --el-table-header-bg-color: rgba(30, 41, 59, 0.5) !important;
  --el-table-row-hover-bg-color: rgba(255, 255, 255, 0.04) !important;
  --el-table-tr-bg-color: transparent !important;
}

.el-table th.el-table__cell {
  background-color: rgba(30, 41, 59, 0.6) !important;
  color: #94a3b8 !important;
  font-weight: 600 !important;
  font-size: 12px !important;
  text-transform: uppercase !important;
  letter-spacing: 0.5px !important;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08) !important;
}

.el-table td.el-table__cell {
  border-bottom: 1px solid rgba(255, 255, 255, 0.05) !important;
  color: #e2e8f0 !important;
}

.el-table--striped .el-table__body tr.el-table__row--striped td.el-table__cell {
  background-color: rgba(255, 255, 255, 0.02) !important;
}

.el-input__wrapper {
  background-color: rgba(30, 41, 59, 0.6) !important;
  box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.1) inset !important;
  border-radius: 8px !important;
  color: #f8fafc !important;
}

.el-input__wrapper.is-focus {
  box-shadow: 0 0 0 1px #38bdf8 inset, 0 0 0 3px rgba(56, 189, 248, 0.2) !important;
}

.el-input__inner {
  color: #f8fafc !important;
}

.el-input__inner::placeholder {
  color: #64748b !important;
}

.el-select-dropdown__item {
  color: #cbd5e1 !important;
}

.el-select-dropdown__item.hover, .el-select-dropdown__item:hover {
  background-color: rgba(56, 189, 248, 0.12) !important;
  color: #38bdf8 !important;
}

.el-dialog {
  background: #0f172a !important;
  border: 1px solid rgba(255, 255, 255, 0.12) !important;
  border-radius: 16px !important;
  color: #f8fafc !important;
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.6) !important;
}

.el-dialog__title {
  color: #f8fafc !important;
  font-weight: 700 !important;
}

.el-dialog__body {
  color: #cbd5e1 !important;
}

.el-form-item__label {
  color: #94a3b8 !important;
  font-weight: 500 !important;
}

.page-header h2 {
  color: #f8fafc !important;
}

.page-header .subtitle {
  color: #94a3b8 !important;
}

/* Accordion / Collapse */
.el-collapse {
  border-top: 1px solid rgba(255, 255, 255, 0.08) !important;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08) !important;
  background: transparent !important;
}

.el-collapse-item__header {
  background: rgba(30, 41, 59, 0.4) !important;
  color: #f1f5f9 !important;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08) !important;
  padding: 0 16px !important;
  font-weight: 600 !important;
  border-radius: 8px !important;
  margin-bottom: 4px !important;
}

.el-collapse-item__wrap {
  background: rgba(15, 23, 42, 0.6) !important;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08) !important;
  padding: 12px 16px !important;
}

.el-collapse-item__content {
  color: #cbd5e1 !important;
}

/* Tabs */
.el-tabs__item {
  color: #94a3b8 !important;
  font-weight: 500 !important;
}

.el-tabs__item.is-active {
  color: #38bdf8 !important;
  font-weight: 700 !important;
}

.el-tabs__active-bar {
  background-color: #38bdf8 !important;
}

.el-tabs__nav-wrap::after {
  background-color: rgba(255, 255, 255, 0.08) !important;
}

/* Dividers */
.el-divider {
  border-color: rgba(255, 255, 255, 0.08) !important;
}

.el-divider__text {
  background-color: #0b0f19 !important;
  color: #94a3b8 !important;
  font-size: 12px !important;
  text-transform: uppercase !important;
  letter-spacing: 0.5px !important;
}

/* Textareas & Inputs */
.el-textarea__inner {
  background-color: rgba(30, 41, 59, 0.6) !important;
  box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.1) inset !important;
  border-radius: 8px !important;
  color: #f8fafc !important;
}

.el-textarea__inner:focus {
  box-shadow: 0 0 0 1px #38bdf8 inset, 0 0 0 3px rgba(56, 189, 248, 0.2) !important;
}

.el-input-number {
  background: transparent !important;
}

.el-input-number .el-input-number__decrease,
.el-input-number .el-input-number__increase {
  background: rgba(30, 41, 59, 0.8) !important;
  color: #94a3b8 !important;
  border-color: rgba(255, 255, 255, 0.1) !important;
}
</style>
