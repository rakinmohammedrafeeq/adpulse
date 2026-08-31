<template>
  <div id="app">
    <!-- Authenticated Layout -->
    <div v-if="authStore.isAuthenticated" class="app-layout">

      <!-- Sidebar -->
      <aside class="sidebar" :class="{ expanded: sidebarExpanded }"
        @mouseenter="sidebarExpanded = true"
        @mouseleave="sidebarExpanded = false"
      >
        <!-- Logo -->
        <div class="sb-logo">
          <router-link to="/" class="brand-link">
            <AdPulseLogo size="small" :tagline="false" :show-text="false" />
            <span class="sb-brand-text">AdPulse</span>
          </router-link>
        </div>

        <!-- Tenant badge -->
        <div class="sb-tenant">
          <span class="live-pulse"></span>
          <span class="sb-tenant-name">{{ authStore.tenantName || 'Acme Corporation' }}</span>
        </div>

        <!-- Nav items -->
        <nav class="sb-nav">
          <router-link to="/" class="sb-item" exact-active-class="sb-item--active">
            <el-icon class="sb-icon"><Odometer /></el-icon>
            <span class="sb-label">Dashboard</span>
          </router-link>
          <router-link to="/campaigns" class="sb-item" active-class="sb-item--active">
            <el-icon class="sb-icon"><TrendCharts /></el-icon>
            <span class="sb-label">Campaigns</span>
          </router-link>
          <router-link to="/audiences" class="sb-item" active-class="sb-item--active">
            <el-icon class="sb-icon"><UserFilled /></el-icon>
            <span class="sb-label">Audiences</span>
          </router-link>
          <router-link to="/analytics" class="sb-item" active-class="sb-item--active">
            <el-icon class="sb-icon"><DataLine /></el-icon>
            <span class="sb-label">Analytics</span>
          </router-link>
          <router-link to="/events" class="sb-item" active-class="sb-item--active">
            <el-icon class="sb-icon"><Lightning /></el-icon>
            <span class="sb-label">Live Feed</span>
          </router-link>
        </nav>

        <!-- Spacer -->
        <div class="sb-spacer"></div>

        <!-- System status -->
        <div class="sb-status">
          <span class="status-dot"></span>
          <span class="sb-label sb-status-label">Attribution Online</span>
        </div>

        <!-- User profile -->
        <div class="sb-user">
          <div class="user-avatar">{{ (authStore.userEmail || 'A')[0].toUpperCase() }}</div>
          <div class="sb-user-meta">
            <span class="user-email">{{ authStore.userEmail || 'admin@acme.com' }}</span>
            <span class="user-role">{{ authStore.userRole || 'Admin' }}</span>
          </div>
        </div>

        <!-- Logout -->
        <button class="sb-logout" @click="confirmLogoutVisible = true">
          <el-icon class="sb-icon"><SwitchButton /></el-icon>
          <span class="sb-label">Sign Out</span>
        </button>
      </aside>

      <!-- Main content -->
      <main class="app-main">
        <router-view />
      </main>
    </div>

    <!-- Unauthenticated (Login) Layout -->
    <div v-else class="auth-layout">
      <router-view />
    </div>

    <!-- Logout Confirmation Modal -->
    <el-dialog
      v-model="confirmLogoutVisible"
      title="Sign Out"
      width="420px"
      align-center
      :close-on-click-modal="false"
    >
      <div class="confirm-body">
        <div class="confirm-icon confirm-icon--warn">
          <el-icon><SwitchButton /></el-icon>
        </div>
        <p class="confirm-text">Are you sure you want to sign out of <strong>AdPulse</strong>?</p>
        <p class="confirm-sub">You will be returned to the login screen.</p>
      </div>
      <template #footer>
        <el-button @click="confirmLogoutVisible = false">Cancel</el-button>
        <el-button type="danger" @click="handleLogout">Yes, Sign Out</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from './stores/auth';
import AdPulseLogo from './components/AdPulseLogo.vue';
import {
  Odometer,
  TrendCharts,
  UserFilled,
  DataLine,
  Lightning,
  SwitchButton
} from '@element-plus/icons-vue';

const router = useRouter();
const authStore = useAuthStore();

const sidebarExpanded = ref(false);
const confirmLogoutVisible = ref(false);

function handleLogout() {
  confirmLogoutVisible.value = false;
  authStore.logout();
  router.push('/login');
}
</script>

<style>
* { box-sizing: border-box; }

body {
  margin: 0;
  padding: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Inter', 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
  background-color: #0b0f19;
  color: #f1f5f9;
  -webkit-font-smoothing: antialiased;
}

#app { min-height: 100vh; }

/* Layout */
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: row;
}

/* Sidebar */
.sidebar {
  width: 64px;
  min-height: 100vh;
  background: rgba(10, 15, 28, 0.98);
  border-right: 1px solid rgba(255, 255, 255, 0.07);
  display: flex;
  flex-direction: column;
  align-items: stretch;
  overflow: hidden;
  transition: width 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  position: fixed;
  top: 0;
  left: 0;
  z-index: 200;
  box-shadow: 2px 0 24px rgba(0, 0, 0, 0.5);
}

.sidebar.expanded { width: 240px; }

/* Logo */
.sb-logo {
  width: 100%;
  padding: 16px 0 14px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
  flex-shrink: 0;
  display: flex;
  justify-content: center;
}

.sidebar.expanded .sb-logo {
  padding: 20px 16px 16px;
  justify-content: flex-start;
}

.brand-link {
  text-decoration: none;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  white-space: nowrap;
  overflow: hidden;
}

.sidebar.expanded .brand-link {
  justify-content: flex-start;
}

.sb-brand-text {
  font-size: 15px;
  font-weight: 800;
  color: #f8fafc;
  opacity: 0;
  max-width: 0;
  overflow: hidden;
  white-space: nowrap;
  transition: opacity 0.18s ease 0.06s, max-width 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}
.sidebar.expanded .sb-brand-text { opacity: 1; max-width: 160px; }

/* Tenant */
.sb-tenant {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 9px;
  padding: 9px 0;
  width: 100%;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  flex-shrink: 0;
  overflow: hidden;
  min-height: 40px;
}

.sidebar.expanded .sb-tenant {
  justify-content: flex-start;
  padding: 11px 18px;
}

.sb-tenant-name {
  font-size: 10px;
  font-weight: 700;
  color: #64748b;
  white-space: nowrap;
  opacity: 0;
  max-width: 0;
  overflow: hidden;
  transition: opacity 0.18s ease 0.06s, max-width 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  letter-spacing: 0.5px;
  text-transform: uppercase;
}
.sidebar.expanded .sb-tenant-name { opacity: 1; max-width: 160px; }

.live-pulse {
  width: 7px;
  height: 7px;
  min-width: 7px;
  background-color: #10b981;
  border-radius: 50%;
  box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7);
  animation: pulse-ring 2s infinite cubic-bezier(0.455, 0.03, 0.515, 0.955);
}

@keyframes pulse-ring {
  0%   { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); }
  70%  { box-shadow: 0 0 0 6px rgba(16, 185, 129, 0); }
  100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
}

/* Nav */
.sb-nav {
  display: flex;
  flex-direction: column;
  gap: 3px;
  width: 100%;
  padding: 10px 0;
  flex-shrink: 0;
}

.sidebar.expanded .sb-nav {
  padding: 12px 10px;
  gap: 4px;
}

.sb-item {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 10px 0;
  border-radius: 10px;
  color: #64748b;
  text-decoration: none;
  font-size: 13px;
  font-weight: 500;
  transition: color 0.15s, background 0.15s;
  white-space: nowrap;
  overflow: hidden;
  width: 100%;
}

.sidebar.expanded .sb-item {
  justify-content: flex-start;
  padding: 11px 14px;
  border-radius: 10px;
}

.sb-item:hover {
  color: #e2e8f0;
  background: rgba(255, 255, 255, 0.06);
}

.sb-item--active {
  color: #38bdf8 !important;
  background: rgba(56, 189, 248, 0.12) !important;
  font-weight: 700 !important;
}

.sb-icon {
  font-size: 18px;
  min-width: 18px;
  flex-shrink: 0;
}

.sb-label {
  opacity: 0;
  max-width: 0;
  overflow: hidden;
  white-space: nowrap;
  transition: opacity 0.18s ease 0.06s, max-width 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}
.sidebar.expanded .sb-label {
  opacity: 1;
  max-width: 160px;
}

.sb-spacer { flex: 1; }

/* Status */
.sb-status {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 9px;
  padding: 8px 0;
  width: 100%;
  overflow: hidden;
  flex-shrink: 0;
}

.sidebar.expanded .sb-status {
  justify-content: flex-start;
  padding: 10px 18px;
}

.status-dot {
  width: 7px;
  height: 7px;
  min-width: 7px;
  border-radius: 50%;
  background: #34d399;
  flex-shrink: 0;
}

.sb-status-label {
  font-size: 11px;
  color: #34d399;
}

/* User */
.sb-user {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  padding: 10px 0;
  width: 100%;
  overflow: hidden;
  border-top: 1px solid rgba(255, 255, 255, 0.06);
  flex-shrink: 0;
}

.sidebar.expanded .sb-user {
  justify-content: flex-start;
  padding: 12px 16px;
}

.user-avatar {
  width: 34px;
  height: 34px;
  min-width: 34px;
  border-radius: 50%;
  background: linear-gradient(135deg, #06b6d4, #3b82f6);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 13px;
  box-shadow: 0 2px 8px rgba(6, 182, 212, 0.3);
  flex-shrink: 0;
}

.sb-user-meta {
  display: flex;
  flex-direction: column;
  line-height: 1.3;
  overflow: hidden;
  opacity: 0;
  max-width: 0;
  white-space: nowrap;
  transition: opacity 0.18s ease 0.06s, max-width 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}
.sidebar.expanded .sb-user-meta { opacity: 1; max-width: 160px; }

.user-email {
  font-size: 11px;
  font-weight: 500;
  color: #e2e8f0;
  overflow: hidden;
  text-overflow: ellipsis;
}

.user-role {
  font-size: 10px;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Logout */
.sb-logout {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 11px 0;
  width: 100%;
  background: none;
  border: none;
  border-top: 1px solid rgba(255, 255, 255, 0.06);
  color: #64748b;
  font-family: inherit;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: color 0.15s, background 0.15s;
  white-space: nowrap;
  overflow: hidden;
  flex-shrink: 0;
}

.sidebar.expanded .sb-logout {
  justify-content: flex-start;
  padding: 13px 18px;
}

.sb-logout:hover {
  color: #f87171;
  background: rgba(239, 68, 68, 0.08);
}

/* Main */
.app-main {
  flex: 1;
  background: #080c14;
  min-height: 100vh;
  margin-left: 64px;
  overflow-x: hidden;
  min-width: 0;
}

/* Confirm dialog */
.confirm-body {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  padding: 12px 0 4px;
  text-align: center;
}

.confirm-icon {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 26px;
  margin-bottom: 4px;
}

.confirm-icon--warn {
  background: rgba(239, 68, 68, 0.14);
  color: #f87171;
  border: 1px solid rgba(239, 68, 68, 0.28);
}

.confirm-icon--danger {
  background: rgba(239, 68, 68, 0.14);
  color: #f87171;
  border: 1px solid rgba(239, 68, 68, 0.28);
}

.confirm-text {
  margin: 0;
  font-size: 15px;
  font-weight: 600;
  color: #f1f5f9;
  line-height: 1.5;
}

.confirm-sub {
  margin: 0;
  font-size: 13px;
  color: #64748b;
}

/* Global dark overrides */
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
.el-input__inner { color: #f8fafc !important; }
.el-input__inner::placeholder { color: #64748b !important; }
.el-select-dropdown__item { color: #cbd5e1 !important; }
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
.el-dialog__title { color: #f8fafc !important; font-weight: 700 !important; }
.el-dialog__body { color: #cbd5e1 !important; }
.el-form-item__label { color: #94a3b8 !important; font-weight: 500 !important; }
.page-header h2 { color: #f8fafc !important; }
.page-header .subtitle { color: #94a3b8 !important; }
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
.el-collapse-item__content { color: #cbd5e1 !important; }
.el-tabs__item { color: #94a3b8 !important; font-weight: 500 !important; }
.el-tabs__item.is-active { color: #38bdf8 !important; font-weight: 700 !important; }
.el-tabs__active-bar { background-color: #38bdf8 !important; }
.el-tabs__nav-wrap::after { background-color: rgba(255, 255, 255, 0.08) !important; }
.el-divider { border-color: rgba(255, 255, 255, 0.08) !important; }
.el-divider__text {
  background-color: #0b0f19 !important;
  color: #94a3b8 !important;
  font-size: 12px !important;
  text-transform: uppercase !important;
  letter-spacing: 0.5px !important;
}
.el-textarea__inner {
  background-color: rgba(30, 41, 59, 0.6) !important;
  box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.1) inset !important;
  border-radius: 8px !important;
  color: #f8fafc !important;
}
.el-textarea__inner:focus {
  box-shadow: 0 0 0 1px #38bdf8 inset, 0 0 0 3px rgba(56, 189, 248, 0.2) !important;
}
.el-input-number { background: transparent !important; }
.el-input-number .el-input-number__decrease,
.el-input-number .el-input-number__increase {
  background: rgba(30, 41, 59, 0.8) !important;
  color: #94a3b8 !important;
  border-color: rgba(255, 255, 255, 0.1) !important;
}
</style>
