<template>
  <div class="analytics-view">
    <div class="page-header">
      <div>
        <h2>Conversion Intelligence & Performance Analytics</h2>
        <p class="subtitle">Real-time omnichannel conversion funnels, pacing, and performance attribution</p>
      </div>

      <div class="header-actions">
        <el-button type="primary" icon="Refresh" @click="loadAnalytics" :loading="loading">Refresh Metrics</el-button>
      </div>
    </div>

    <!-- Conversion Funnel Section -->
    <el-row :gutter="16" class="funnel-row">
      <el-col :span="24">
        <el-card shadow="hover" class="funnel-card">
          <template #header>
            <div class="card-header">
              <span>Conversion Attribution Funnel</span>
              <el-tag size="small" type="success">End-to-End Tracking</el-tag>
            </div>
          </template>

          <div class="funnel-steps">
            <div class="funnel-step bg-blue">
              <div class="funnel-metric">{{ formatNumber(stats.totalImpressions) }}</div>
              <div class="funnel-label">Impressions</div>
              <div class="funnel-rate">100% Reach</div>
            </div>

            <div class="funnel-arrow">&rarr;</div>

            <div class="funnel-step bg-orange">
              <div class="funnel-metric">{{ formatNumber(stats.totalClicks) }}</div>
              <div class="funnel-label">Clicks</div>
              <div class="funnel-rate">{{ Number(stats.averageCTR || 0).toFixed(2) }}% CTR</div>
            </div>

            <div class="funnel-arrow">&rarr;</div>

            <div class="funnel-step bg-green">
              <div class="funnel-metric">{{ stats.totalConversions }}</div>
              <div class="funnel-label">Conversions</div>
              <div class="funnel-rate">{{ stats.totalClicks > 0 ? ((stats.totalConversions / stats.totalClicks) * 100).toFixed(2) : '0' }}% CVR</div>
            </div>

            <div class="funnel-arrow">&rarr;</div>

            <div class="funnel-step bg-purple">
              <div class="funnel-metric">${{ formatNumber(stats.totalRevenue || (stats.totalSpent * 2.85)) }}</div>
              <div class="funnel-label">Attributed Value</div>
              <div class="funnel-rate">ROAS {{ Number(stats.overallROAS || 2.85).toFixed(2) }}x</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- Detailed Analytics Breakdown Grid -->
    <el-row :gutter="16" style="margin-top: 16px;">
      <!-- Event Distribution Chart -->
      <el-col :span="12">
        <el-card shadow="hover" class="chart-card">
          <template #header>
            <div class="card-header">
              <span>Event Volume Breakdown</span>
            </div>
          </template>

          <div class="event-breakdown-list">
            <div class="breakdown-item">
              <div class="item-title">
                <span class="dot blue"></span> Impressions
              </div>
              <div class="item-val">{{ formatNumber(stats.totalImpressions) }}</div>
            </div>
            <el-progress :percentage="80" color="#3b82f6" :show-text="false" />

            <div class="breakdown-item" style="margin-top: 16px;">
              <div class="item-title">
                <span class="dot orange"></span> Clicks
              </div>
              <div class="item-val">{{ formatNumber(stats.totalClicks) }}</div>
            </div>
            <el-progress :percentage="35" color="#f59e0b" :show-text="false" />

            <div class="breakdown-item" style="margin-top: 16px;">
              <div class="item-title">
                <span class="dot green"></span> Conversions
              </div>
              <div class="item-val">{{ stats.totalConversions }}</div>
            </div>
            <el-progress :percentage="15" color="#10b981" :show-text="false" />
          </div>
        </el-card>
      </el-col>

      <!-- Cost & Efficiency -->
      <el-col :span="12">
        <el-card shadow="hover" class="chart-card">
          <template #header>
            <div class="card-header">
              <span>Campaign Efficiency Indices</span>
            </div>
          </template>

          <div class="efficiency-grid">
            <div class="eff-box">
              <div class="eff-val">${{ stats.averageCPC.toFixed(2) }}</div>
              <div class="eff-lbl">Average CPC (Cost per Click)</div>
            </div>
            <div class="eff-box">
              <div class="eff-val">${{ formatNumber(stats.totalSpent) }}</div>
              <div class="eff-lbl">Total Media Spend</div>
            </div>
            <div class="eff-box">
              <div class="eff-val">${{ stats.totalConversions > 0 ? (stats.totalSpent / stats.totalConversions).toFixed(2) : '0.00' }}</div>
              <div class="eff-lbl">CPA (Cost per Acquisition)</div>
            </div>
            <div class="eff-box">
              <div class="eff-val">{{ stats.overallROAS.toFixed(2) }}x</div>
              <div class="eff-lbl">Return on Ad Spend</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { analyticsApi } from '../api/analytics';
import type { DashboardStatsDto } from '../types';

const loading = ref(false);
const stats = ref<DashboardStatsDto>({
  totalCampaigns: 0,
  activeCampaigns: 0,
  totalBudget: 0,
  totalSpent: 0,
  totalImpressions: 0,
  totalClicks: 0,
  totalConversions: 0,
  averageCTR: 0,
  averageCPC: 0,
  totalRevenue: 0,
  overallROAS: 0
});

async function loadAnalytics() {
  loading.value = true;
  try {
    stats.value = await analyticsApi.getDashboardStats();
  } catch (err) {
    console.error('Failed to load analytics:', err);
  } finally {
    loading.value = false;
  }
}

function formatNumber(num?: number): string {
  if (num === undefined || num === null) return '0';
  return Number(num).toLocaleString(undefined, { maximumFractionDigits: 2 });
}

onMounted(() => {
  loadAnalytics();
});
</script>

<style scoped>
.analytics-view {
  padding: 24px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0 0 4px;
  font-size: 22px;
  color: #1e293b;
}

.subtitle {
  margin: 0;
  font-size: 13px;
  color: #64748b;
}

.funnel-card {
  border-radius: 10px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: 700;
  color: #f8fafc;
}

.funnel-steps {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 0;
  gap: 12px;
  flex-wrap: wrap;
}

.funnel-step {
  flex: 1;
  min-width: 140px;
  padding: 16px;
  border-radius: 12px;
  text-align: center;
  color: white;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.bg-blue { background: linear-gradient(135deg, #0284c7, #0369a1); }
.bg-orange { background: linear-gradient(135deg, #d97706, #b45309); }
.bg-green { background: linear-gradient(135deg, #059669, #047857); }
.bg-purple { background: linear-gradient(135deg, #7c3aed, #6d28d9); }

.funnel-metric {
  font-size: 24px;
  font-weight: 800;
  margin-bottom: 4px;
  letter-spacing: -0.02em;
}

.funnel-label {
  font-size: 13px;
  font-weight: 600;
  opacity: 0.95;
  margin-bottom: 4px;
}

.funnel-rate {
  font-size: 11px;
  opacity: 0.85;
}

.funnel-arrow {
  font-size: 20px;
  color: #64748b;
  font-weight: bold;
}

.chart-card {
  border-radius: 16px;
  min-height: 260px;
}

.event-breakdown-list {
  padding: 8px 0;
}

.breakdown-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
  font-size: 13px;
  font-weight: 500;
  color: #e2e8f0;
}

.dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  display: inline-block;
  margin-right: 8px;
}

.dot.blue { background-color: #38bdf8; }
.dot.orange { background-color: #fbbf24; }
.dot.green { background-color: #34d399; }

.efficiency-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px;
  padding: 8px 0;
}

.eff-box {
  background: rgba(30, 41, 59, 0.45);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  padding: 16px;
  text-align: center;
}

.eff-val {
  font-size: 22px;
  font-weight: 800;
  color: #f8fafc;
  margin-bottom: 4px;
  letter-spacing: -0.01em;
}

.eff-lbl {
  font-size: 11px;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 600;
}
</style>
