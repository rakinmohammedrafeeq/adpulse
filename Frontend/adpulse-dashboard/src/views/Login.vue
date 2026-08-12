<template>
  <div class="login-page">
    <!-- Ambient Background Glows -->
    <div class="ambient-glow glow-top"></div>
    <div class="ambient-glow glow-bottom"></div>

    <div class="login-container">
      <!-- Left Side: Product Showcase (Gen Z / Modern SaaS) -->
      <div class="product-showcase">
        <div class="brand-top">
          <AdPulseLogo size="large" :tagline="true" />
        </div>

        <div class="hero-content">
          <div class="hero-pill">
            <span class="live-dot"></span>
            <span>Next-Gen Advertising Intelligence</span>
          </div>

          <h1 class="hero-headline">
            Campaign Intelligence <br />
            <span class="gradient-text">Engineered for Velocity.</span>
          </h1>

          <p class="hero-subtext">
            Ingest millions of ad signals with sub-millisecond latency. Optimize omnichannel spend, verify attribution integrity, and unlock predictive audience cohorts in real time.
          </p>

          <div class="feature-pills">
            <div class="feat-item">
              <span class="feat-check">&check;</span>
              <span>Hardware-Accelerated Ingestion</span>
            </div>
            <div class="feat-item">
              <span class="feat-check">&check;</span>
              <span>Strict Multi-Tenant Isolation</span>
            </div>
            <div class="feat-item">
              <span class="feat-check">&check;</span>
              <span>Sub-Second Dimensional Analytics</span>
            </div>
          </div>
        </div>

        <div class="hero-testimonial">
          <p class="quote">"AdPulse gave our marketing engineering team real-time visibility into attribution fraud and campaign pacing."</p>
          <div class="author">
            <span class="author-name">Alex Rivera</span>
            <span class="author-title">&bull; VP of Growth Engineering</span>
          </div>
        </div>
      </div>

      <!-- Right Side: Interactive Auth Box -->
      <div class="auth-card-panel">
        <div class="card-glass">
          <div class="card-intro">
            <h2 class="auth-title">Access Command Center</h2>
            <p class="auth-subtitle">Sign in to your enterprise tenant workspace</p>
          </div>

          <el-tabs v-model="activeTab" class="dark-tabs">
            <!-- SIGN IN TAB -->
            <el-tab-pane label="Sign In" name="login">
              <el-form
                :model="loginForm"
                :rules="loginRules"
                ref="loginFormRef"
                label-position="top"
                @submit.prevent="handleLogin"
                class="auth-form"
              >
                <el-form-item label="Enterprise Email" prop="email">
                  <el-input
                    v-model="loginForm.email"
                    placeholder="admin@acme.com"
                    prefix-icon="User"
                    class="dark-input"
                  />
                </el-form-item>

                <el-form-item label="Security Password" prop="password">
                  <el-input
                    v-model="loginForm.password"
                    type="password"
                    placeholder="••••••••"
                    show-password
                    prefix-icon="Lock"
                    class="dark-input"
                  />
                </el-form-item>

                <!-- One-click Demo Credentials Helper -->
                <div class="demo-auto-pill" @click="fillDemoCredentials">
                  <div class="demo-pill-left">
                    <span class="lightning-icon">&#9889;</span>
                    <span>Demo Account: <strong>Acme Corporation</strong></span>
                  </div>
                  <span class="demo-badge">Click to Fill</span>
                </div>

                <el-button
                  type="primary"
                  class="submit-action-btn"
                  :loading="loading"
                  @click="handleLogin"
                >
                  Enter Platform &rarr;
                </el-button>
              </el-form>
            </el-tab-pane>

            <!-- REGISTER TENANT TAB -->
            <el-tab-pane label="Provision Tenant" name="register">
              <el-form
                :model="regForm"
                :rules="regRules"
                ref="regFormRef"
                label-position="top"
                @submit.prevent="handleRegister"
                class="auth-form"
              >
                <el-row :gutter="12">
                  <el-col :span="12">
                    <el-form-item label="Tenant Slug" prop="tenantName">
                      <el-input v-model="regForm.tenantName" placeholder="apex-media" class="dark-input" />
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="Company" prop="companyName">
                      <el-input v-model="regForm.companyName" placeholder="Apex Media Inc" class="dark-input" />
                    </el-form-item>
                  </el-col>
                </el-row>

                <el-row :gutter="12">
                  <el-col :span="12">
                    <el-form-item label="First Name" prop="firstName">
                      <el-input v-model="regForm.firstName" placeholder="Sarah" class="dark-input" />
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="Last Name" prop="lastName">
                      <el-input v-model="regForm.lastName" placeholder="Connor" class="dark-input" />
                    </el-form-item>
                  </el-col>
                </el-row>

                <el-form-item label="Admin Work Email" prop="email">
                  <el-input v-model="regForm.email" placeholder="sarah@apex.com" prefix-icon="User" class="dark-input" />
                </el-form-item>

                <el-form-item label="Password" prop="password">
                  <el-input v-model="regForm.password" type="password" placeholder="Min 8 characters" show-password prefix-icon="Lock" class="dark-input" />
                </el-form-item>

                <el-button
                  type="primary"
                  class="submit-action-btn register-gradient"
                  :loading="loading"
                  @click="handleRegister"
                >
                  Provision Isolated Tenant
                </el-button>
              </el-form>
            </el-tab-pane>
          </el-tabs>

          <div class="card-footer-note">
            <span>Verified Local Environment &bull; Demo Mode</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import { useAuthStore } from '../stores/auth';
import AdPulseLogo from '../components/AdPulseLogo.vue';

const router = useRouter();
const authStore = useAuthStore();

const activeTab = ref<'login' | 'register'>('login');
const loading = ref(false);

const loginFormRef = ref<FormInstance>();
const regFormRef = ref<FormInstance>();

const loginForm = reactive({
  email: 'admin@acme.com',
  password: 'Password123!'
});

const regForm = reactive({
  tenantName: '',
  companyName: '',
  email: '',
  password: '',
  firstName: '',
  lastName: ''
});

const loginRules: FormRules = {
  email: [{ required: true, message: 'Please enter your email', trigger: 'blur' }],
  password: [{ required: true, message: 'Please enter your password', trigger: 'blur' }]
};

const regRules: FormRules = {
  tenantName: [{ required: true, message: 'Tenant slug required', trigger: 'blur' }],
  companyName: [{ required: true, message: 'Company name required', trigger: 'blur' }],
  email: [{ required: true, type: 'email', message: 'Valid email required', trigger: 'blur' }],
  password: [{ required: true, min: 8, message: 'Minimum 8 characters required', trigger: 'blur' }],
  firstName: [{ required: true, message: 'First name required', trigger: 'blur' }],
  lastName: [{ required: true, message: 'Last name required', trigger: 'blur' }]
};

function fillDemoCredentials() {
  loginForm.email = 'admin@acme.com';
  loginForm.password = 'Password123!';
  ElMessage.success('Loaded demo credentials for Acme Corporation');
}

async function handleLogin() {
  if (!loginFormRef.value) return;
  await loginFormRef.value.validate(async (valid) => {
    if (!valid) return;
    loading.value = true;
    try {
      const success = await authStore.login(loginForm.email, loginForm.password);
      if (success) {
        ElMessage.success('Signed in to AdPulse platform.');
        router.push('/');
      } else {
        ElMessage.error('Invalid email or password.');
      }
    } finally {
      loading.value = false;
    }
  });
}

async function handleRegister() {
  if (!regFormRef.value) return;
  await regFormRef.value.validate(async (valid) => {
    if (!valid) return;
    loading.value = true;
    try {
      const success = await authStore.register(regForm);
      if (success) {
        ElMessage.success('Tenant provisioned successfully!');
        router.push('/');
      } else {
        ElMessage.error('Registration failed. Tenant slug or email may already exist.');
      }
    } finally {
      loading.value = false;
    }
  });
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  background-color: #080c14;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
  padding: 40px 20px;
}

/* Ambient Background Lights */
.ambient-glow {
  position: absolute;
  width: 600px;
  height: 600px;
  border-radius: 50%;
  filter: blur(140px);
  opacity: 0.25;
  pointer-events: none;
}

.glow-top {
  top: -150px;
  left: -100px;
  background: radial-gradient(circle, #06b6d4, transparent 70%);
}

.glow-bottom {
  bottom: -150px;
  right: -100px;
  background: radial-gradient(circle, #8b5cf6, transparent 70%);
}

.login-container {
  display: grid;
  grid-template-columns: 1.2fr 1fr;
  max-width: 1140px;
  width: 100%;
  gap: 48px;
  align-items: center;
  position: relative;
  z-index: 10;
}

@media (max-width: 900px) {
  .login-container {
    grid-template-columns: 1fr;
    max-width: 500px;
  }
  .product-showcase {
    display: none;
  }
}

/* Showcase Column */
.product-showcase {
  display: flex;
  flex-direction: column;
  gap: 32px;
  padding-right: 20px;
}

.hero-pill {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: rgba(6, 182, 212, 0.12);
  border: 1px solid rgba(6, 182, 212, 0.3);
  padding: 6px 14px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  color: #38bdf8;
  margin-bottom: 16px;
}

.live-dot {
  width: 6px;
  height: 6px;
  background: #38bdf8;
  border-radius: 50%;
  box-shadow: 0 0 8px #38bdf8;
}

.hero-headline {
  font-size: 38px;
  font-weight: 800;
  line-height: 1.15;
  color: #f8fafc;
  letter-spacing: -1px;
  margin: 0 0 16px;
}

.gradient-text {
  background: linear-gradient(135deg, #38bdf8 0%, #818cf8 50%, #c084fc 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.hero-subtext {
  font-size: 15px;
  line-height: 1.6;
  color: #94a3b8;
  margin: 0 0 24px;
}

.feature-pills {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.feat-item {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 13px;
  font-weight: 500;
  color: #cbd5e1;
}

.feat-check {
  width: 20px;
  height: 20px;
  background: rgba(16, 185, 129, 0.2);
  color: #34d399;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 700;
}

.hero-testimonial {
  background: rgba(30, 41, 59, 0.4);
  border-left: 3px solid #38bdf8;
  padding: 16px 20px;
  border-radius: 0 12px 12px 0;
}

.quote {
  font-size: 13px;
  font-style: italic;
  color: #e2e8f0;
  margin: 0 0 8px;
}

.author {
  font-size: 12px;
  color: #94a3b8;
}

.author-name {
  font-weight: 600;
  color: #f1f5f9;
}

/* Card Panel */
.auth-card-panel {
  display: flex;
  justify-content: center;
}

.card-glass {
  background: rgba(15, 23, 42, 0.75);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 32px 28px;
  width: 100%;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.5);
}

.card-intro {
  margin-bottom: 24px;
}

.auth-title {
  font-size: 22px;
  font-weight: 700;
  color: #f8fafc;
  margin: 0 0 6px;
}

.auth-subtitle {
  font-size: 13px;
  color: #94a3b8;
  margin: 0;
}

.demo-auto-pill {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(56, 189, 248, 0.08);
  border: 1px solid rgba(56, 189, 248, 0.25);
  border-radius: 10px;
  padding: 8px 12px;
  margin-bottom: 20px;
  cursor: pointer;
  transition: all 0.2s ease;
  color: #e2e8f0;
  font-size: 12px;
}

.demo-auto-pill:hover {
  background: rgba(56, 189, 248, 0.16);
  border-color: rgba(56, 189, 248, 0.5);
  transform: scale(1.01);
}

.lightning-icon {
  color: #f59e0b;
  margin-right: 6px;
}

.demo-badge {
  background: rgba(56, 189, 248, 0.2);
  color: #38bdf8;
  font-weight: 600;
  font-size: 11px;
  padding: 2px 8px;
  border-radius: 12px;
}

.submit-action-btn {
  width: 100%;
  height: 44px;
  font-size: 14px;
  font-weight: 700;
  border-radius: 10px;
  border: none;
  background: linear-gradient(135deg, #06b6d4 0%, #3b82f6 100%) !important;
  box-shadow: 0 4px 16px rgba(6, 182, 212, 0.35) !important;
  transition: all 0.2s ease;
}

.submit-action-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 20px rgba(6, 182, 212, 0.45) !important;
}

.register-gradient {
  background: linear-gradient(135deg, #10b981 0%, #06b6d4 100%) !important;
  box-shadow: 0 4px 16px rgba(16, 185, 129, 0.35) !important;
}

.card-footer-note {
  text-align: center;
  margin-top: 24px;
  font-size: 11px;
  color: #64748b;
}

/* Tabs & Input Dark Overrides */
:deep(.el-tabs__nav-wrap::after) {
  background-color: rgba(255, 255, 255, 0.08) !important;
}

:deep(.el-tabs__item) {
  color: #94a3b8 !important;
  font-weight: 600 !important;
}

:deep(.el-tabs__item.is-active) {
  color: #38bdf8 !important;
}

:deep(.el-tabs__active-bar) {
  background-color: #38bdf8 !important;
}

:deep(.el-form-item__label) {
  color: #cbd5e1 !important;
  font-size: 12px !important;
  font-weight: 500 !important;
  padding-bottom: 4px !important;
}

:deep(.el-input__wrapper) {
  background-color: rgba(30, 41, 59, 0.6) !important;
  box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.1) inset !important;
  border-radius: 8px !important;
}

:deep(.el-input__wrapper.is-focus) {
  box-shadow: 0 0 0 1px #38bdf8 inset, 0 0 0 3px rgba(56, 189, 248, 0.2) !important;
}

:deep(.el-input__inner) {
  color: #f8fafc !important;
}
</style>
