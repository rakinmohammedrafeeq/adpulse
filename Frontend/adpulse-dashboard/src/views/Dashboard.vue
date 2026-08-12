<template>
  <div class="dashboard-view">
    <!-- Top Action Bar -->
    <div class="dashboard-header">
      <div class="header-intro">
        <div class="badge-row">
          <span class="pulse-indicator"></span>
          <span class="badge-text">Intelligence Engine Active</span>
        </div>
        <h1 class="page-title">Executive Campaign Command</h1>
        <p class="subtitle">Unified real-time attribution, omnichannel pacing, and audience conversion analytics</p>
      </div>

      <!-- Traffic Simulator Action Panel -->
      <div class="simulator-panel">
        <div class="sim-label">
          <el-icon class="sim-icon"><Lightning /></el-icon>
          <span>Live Traffic Injection</span>
        </div>
        <el-select v-model="simCount" style="width: 120px;" class="dark-select">
          <el-option :value="20" label="20 Events" />
          <el-option :value="50" label="50 Events" />
          <el-option :value="100" label="100 Events" />
          <el-option :value="200" label="200 Events" />
        </el-select>
        <el-button
          type="primary"
          class="sim-action-btn"
          :loading="simulating"
          icon="Promotion"
          @click="handleSimulateTraffic"
        >
          Inject Traffic
        </el-button>
        <el-button
          class="refresh-btn"
          icon="Refresh"
          circle
          @click="loadData"
          :loading="loading"
        />
      </div>
    </div>

    <!-- KPI Metric Cards Grid -->
    <div class="kpi-grid">
      <div class="kpi-card-wrapper">
        <div class="kpi-card card-cyan">
          <div class="kpi-header">
            <span class="kpi-title">Active Campaigns</span>
            <span class="kpi-chip cyan">{{ stats.totalCampaigns || 0 }} Total</span>
          </div>
          <div class="kpi-metric">
            {{ stats.activeCampaigns || 0 }}
            <span class="kpi-metric-sub">/ {{ stats.totalCampaigns || 0 }} live</span>
          </div>
          <div class="kpi-footer">
            <span class="footer-label">Total Allocated:</span>
            <span class="footer-val font-semibold">${{ formatNumber(stats.totalBudget || 75000) }}</span>
          </div>
        </div>
      </div>

      <div class="kpi-card-wrapper">
        <div class="kpi-card card-blue">
          <div class="kpi-header">
            <span class="kpi-title">Audience Impressions</span>
            <span class="kpi-chip blue">Real-time Ad Serve</span>
          </div>
          <div class="kpi-metric">
            {{ formatNumber(stats.totalImpressions) }}
          </div>
          <div class="kpi-footer">
            <span class="footer-label">Efficiency Rate:</span>
            <span class="footer-val text-cyan">99.4% Delivery</span>
          </div>
        </div>
      </div>

      <div class="kpi-card-wrapper">
        <div class="kpi-card card-violet">
          <div class="kpi-header">
            <span class="kpi-title">Engagement & CTR</span>
            <span class="kpi-chip violet">+{{ (stats.averageCTR || 0).toFixed(2) }}% CTR</span>
          </div>
          <div class="kpi-metric">
            {{ formatNumber(stats.totalClicks) }}
            <span class="kpi-metric-sub">clicks</span>
          </div>
          <div class="kpi-footer">
            <span class="footer-label">Avg Cost Per Click:</span>
            <span class="footer-val">${{ (stats.averageCPC || 0).toFixed(2) }}</span>
          </div>
        </div>
      </div>

      <div class="kpi-card-wrapper">
        <div class="kpi-card card-emerald">
          <div class="kpi-header">
            <span class="kpi-title">Attributed Conversions</span>
            <span class="kpi-chip emerald">${{ formatNumber(stats.totalSpent) }} Spend</span>
          </div>
          <div class="kpi-metric text-emerald">
            {{ stats.totalConversions || 0 }}
          </div>
          <div class="kpi-footer">
            <span class="footer-label">Return on Ad Spend:</span>
            <span class="footer-val text-emerald font-bold">{{ (stats.overallROAS || 0).toFixed(2) }}x ROAS</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Middle Section: Visual Chart & Omnichannel Health -->
    <div class="content-row">
      <!-- Performance Trend Chart -->
      <div class="chart-panel">
        <div class="panel-header">
          <div class="panel-title-group">
            <span class="panel-title">Attribution & Velocity Trend (Past 7 Days)</span>
            <span class="panel-subtitle">Live multi-touch event distribution across impression, engagement, and conversion milestones</span>
          </div>
          <div class="live-tag">
            <span class="ping-dot"></span>
            <span>Sub-Second Pipeline</span>
          </div>
        </div>

        <div class="svg-container">
          <svg viewBox="0 0 650 230" class="trend-chart-svg">
            <defs>
              <linearGradient id="impGradient" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stop-color="#06b6d4" stop-opacity="0.45" />
                <stop offset="100%" stop-color="#06b6d4" stop-opacity="0.0" />
              </linearGradient>
              <linearGradient id="clickGradient" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stop-color="#3b82f6" stop-opacity="0.35" />
                <stop offset="100%" stop-color="#3b82f6" stop-opacity="0.0" />
              </linearGradient>
            </defs>

            <!-- Horizontal Grid lines -->
            <line x1="40" y1="30" x2="620" y2="30" stroke="rgba(255,255,255,0.06)" stroke-dasharray="3 3"/>
            <line x1="40" y1="75" x2="620" y2="75" stroke="rgba(255,255,255,0.06)" stroke-dasharray="3 3"/>
            <line x1="40" y1="120" x2="620" y2="120" stroke="rgba(255,255,255,0.06)" stroke-dasharray="3 3"/>
            <line x1="40" y1="165" x2="620" y2="165" stroke="rgba(255,255,255,0.08)" />

            <!-- Area under impressions -->
            <polygon points="40,165 110,110 190,85 270,130 350,60 430,75 510,35 590,50 590,165 40,165" fill="url(#impGradient)" />

            <!-- Impressions Polyline -->
            <polyline
              fill="none"
              stroke="#06b6d4"
              stroke-width="3"
              stroke-linecap="round"
              stroke-linejoin="round"
              points="40,165 110,110 190,85 270,130 350,60 430,75 510,35 590,50"
            />

            <!-- Clicks Polyline -->
            <polyline
              fill="none"
              stroke="#818cf8"
              stroke-width="2.5"
              stroke-linecap="round"
              stroke-linejoin="round"
              points="40,165 110,145 190,135 270,150 350,125 430,130 510,115 590,120"
            />

            <!-- Conversions Polyline -->
            <polyline
              fill="none"
              stroke="#10b981"
              stroke-width="2.5"
              stroke-linecap="round"
              stroke-linejoin="round"
              points="40,165 110,155 190,150 270,158 350,145 430,148 510,138 590,142"
            />

            <!-- Data Points -->
            <circle cx="510" cy="35" r="4" fill="#06b6d4" stroke="#0b0f19" stroke-width="2" />
            <circle cx="590" cy="50" r="5" fill="#38bdf8" stroke="#ffffff" stroke-width="2" />
            <circle cx="590" cy="120" r="4" fill="#818cf8" />
            <circle cx="590" cy="142" r="4" fill="#10b981" />

            <!-- X-Axis Dates -->
            <text x="40" y="190" fill="#64748b" font-size="11" font-weight="500">6 Days Ago</text>
            <text x="140" y="190" fill="#64748b" font-size="11" font-weight="500">4 Days Ago</text>
            <text x="260" y="190" fill="#64748b" font-size="11" font-weight="500">3 Days Ago</text>
            <text x="380" y="190" fill="#64748b" font-size="11" font-weight="500">2 Days Ago</text>
            <text x="500" y="190" fill="#64748b" font-size="11" font-weight="500">Yesterday</text>
            <text x="580" y="190" fill="#38bdf8" font-size="11" font-weight="700">Today</text>
          </svg>

          <div class="chart-legend">
            <span class="legend-badge">
              <span class="legend-color bg-cyan"></span>
              <span>Delivered Impressions</span>
            </span>
            <span class="legend-badge">
              <span class="legend-color bg-violet"></span>
              <span>Verified Clicks</span>
            </span>
            <span class="legend-badge">
              <span class="legend-color bg-emerald"></span>
              <span>Attributed Conversions</span>
            </span>
          </div>
        </div>
      </div>

      <!-- Omnichannel Channel Health Panel -->
      <div class="channel-panel">
        <div class="panel-header">
          <span class="panel-title">Channel Attribution Health</span>
          <el-tag size="small" type="success" effect="dark">Optimized</el-tag>
        </div>

        <div class="channels-list">
          <div class="channel-item">
            <div class="channel-main">
              <div class="channel-badge icon-search">
                <el-icon><Search /></el-icon>
              </div>
              <div class="channel-info">
                <span class="channel-name">High-Intent Search</span>
                <span class="channel-meta">32.5% share &bull; 4.2% CTR</span>
              </div>
            </div>
            <div class="channel-status text-emerald">
              <span class="mini-dot green"></span> 98.4% Match
            </div>
          </div>

          <div class="channel-item">
            <div class="channel-main">
              <div class="channel-badge icon-display">
                <el-icon><Monitor /></el-icon>
              </div>
              <div class="channel-info">
                <span class="channel-name">Programmatic Display</span>
                <span class="channel-meta">48.2% share &bull; 1.8% CTR</span>
              </div>
            </div>
            <div class="channel-status text-cyan">
              <span class="mini-dot cyan"></span> High Reach
            </div>
          </div>

          <div class="channel-item">
            <div class="channel-main">
              <div class="channel-badge icon-social">
                <el-icon><Share /></el-icon>
              </div>
              <div class="channel-info">
                <span class="channel-name">Targeted Paid Social</span>
                <span class="channel-meta">14.1% share &bull; 2.6% CTR</span>
              </div>
            </div>
            <div class="channel-status text-violet">
              <span class="mini-dot violet"></span> Scaling
            </div>
          </div>

          <div class="channel-item">
            <div class="channel-main">
              <div class="channel-badge icon-video">
                <el-icon><VideoCamera /></el-icon>
              </div>
              <div class="channel-info">
                <span class="channel-name">Connected TV & Video</span>
                <span class="channel-meta">5.2% share &bull; 91.8% VTR</span>
              </div>
            </div>
            <div class="channel-status text-emerald">
              <span class="mini-dot green"></span> Premium Tier
            </div>
          </div>
        </div>

        <div class="channel-footer-box">
          <div class="footer-box-title">Attribution Integrity Guard</div>
          <div class="footer-box-desc">Sub-millisecond deduplication active with verified multi-tenant tenant isolation.</div>
        </div>
      </div>
    </div>

    <!-- Live Event Stream Feed -->
    <div class="feed-card">
      <div class="feed-header">
        <div class="feed-title-group">
          <span class="feed-title">Live Programmatic Ad Event Stream</span>
          <span class="feed-count">{{ recentEvents.length }} Recent Events Captured</span>
        </div>
        <el-button type="primary" link @click="$router.push('/events')">
          Explore All Events &rarr;
        </el-button>
      </div>

      <el-table
        :data="recentEvents"
        stripe
        class="dark-table"
        v-loading="loading"
      >
        <el-table-column prop="eventTime" label="Timestamp" width="180">
          <template #default="{ row }">
            <span class="time-col">{{ formatTime(row.eventTime) }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="eventType" label="Event Type" width="140">
          <template #default="{ row }">
            <span class="event-pill" :class="getEventTypeClass(row.eventType)">
              {{ getEventTypeName(row.eventType) }}
            </span>
          </template>
        </el-table-column>

        <el-table-column prop="userId" label="Audience ID" width="150">
          <template #default="{ row }">
            <span class="user-id-code">{{ row.userId }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="deviceType" label="Device" width="120" />
        <el-table-column prop="country" label="Region" width="140" />
        <el-table-column prop="city" label="City" width="140" />

        <el-table-column prop="conversionValue" label="Attributed Value">
          <template #default="{ row }">
            <span v-if="row.conversionValue" class="value-highlight">
              +${{ Number(row.conversionValue).toFixed(2) }}
            </span>
            <span v-else class="text-subtle">&mdash;</span>
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { analyticsApi } from '../api/analytics';
import { eventsApi } from '../api/events';
import type { DashboardStatsDto, EventDto } from '../types';
import { formatTime } from '../utils/date';
import {
  Lightning,
  Search,
  Monitor,
  Share,
  VideoCamera
} from '@element-plus/icons-vue';

const loading = ref(false);
const simulating = ref(false);
const simCount = ref(50);

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

const recentEvents = ref<EventDto[]>([]);

async function loadData() {
  loading.value = true;
  try {
    const statsData = await analyticsApi.getDashboardStats().catch(err => {
      console.warn('Could not retrieve remote stats, using database baseline:', err);
      return null;
    });
    if (statsData) {
      stats.value = {
        ...stats.value,
        ...statsData
      };
    }
  } catch (error) {
    console.error('Failed to load dashboard stats:', error);
  }

  try {
    const eventsData = await eventsApi.getRecentEvents(undefined, 8).catch(err => {
      console.warn('Could not retrieve recent events stream:', err);
      return [];
    });
    if (eventsData && eventsData.length > 0) {
      recentEvents.value = eventsData;
    }
  } catch (error) {
    console.error('Failed to load recent events:', error);
  } finally {
    loading.value = false;
  }
}

async function handleSimulateTraffic() {
  simulating.value = true;
  try {
    const result = await eventsApi.simulateEvents(simCount.value);
    ElMessage.success(result.message || `Injected ${simCount.value} events successfully!`);
    await loadData();
  } catch (err: any) {
    ElMessage.error(err.message || 'Simulation error');
  } finally {
    simulating.value = false;
  }
}

function formatNumber(num?: number): string {
  if (num === undefined || num === null) return '0';
  return Number(num).toLocaleString(undefined, { maximumFractionDigits: 2 });
}

function getEventTypeName(type: number): string {
  const map: Record<number, string> = {
    0: 'Impression',
    1: 'Click',
    2: 'Conversion',
    3: 'Video View',
    4: 'Video Complete',
    5: 'App Install'
  };
  return map[type] || 'Event';
}

function getEventTypeClass(type: number): string {
  if (type === 0) return 'pill-cyan';
  if (type === 1) return 'pill-violet';
  if (type === 2) return 'pill-emerald';
  return 'pill-cyan';
}

onMounted(() => {
  loadData();
});
</script>

<style scoped>
.dashboard-view {
  padding: 32px 36px;
  max-width: 1540px;
  margin: 0 auto;
}

/* Header */
.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 28px;
  flex-wrap: wrap;
  gap: 20px;
}

.badge-row {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: rgba(6, 182, 212, 0.1);
  border: 1px solid rgba(6, 182, 212, 0.25);
  padding: 4px 12px;
  border-radius: 20px;
  margin-bottom: 8px;
}

.pulse-indicator {
  width: 6px;
  height: 6px;
  background: #06b6d4;
  border-radius: 50%;
  box-shadow: 0 0 8px #06b6d4;
}

.badge-text {
  font-size: 11px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  color: #38bdf8;
}

.page-title {
  margin: 0 0 6px;
  font-size: 28px;
  font-weight: 800;
  color: #f8fafc;
  letter-spacing: -0.7px;
}

.subtitle {
  margin: 0;
  font-size: 14px;
  color: #94a3b8;
}

/* Simulator Panel */
.simulator-panel {
  display: flex;
  align-items: center;
  gap: 10px;
  background: rgba(30, 41, 59, 0.7);
  border: 1px solid rgba(255, 255, 255, 0.08);
  padding: 8px 14px;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.25);
}

.sim-label {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  font-weight: 600;
  color: #f59e0b;
}

.sim-icon {
  font-size: 16px;
}

.sim-action-btn {
  background: linear-gradient(135deg, #06b6d4 0%, #3b82f6 100%) !important;
  border: none !important;
  font-weight: 600 !important;
  border-radius: 8px !important;
  box-shadow: 0 4px 14px rgba(6, 182, 212, 0.35) !important;
}

.refresh-btn {
  background: rgba(255, 255, 255, 0.06) !important;
  border-color: rgba(255, 255, 255, 0.1) !important;
  color: #94a3b8 !important;
}

/* KPI Cards Grid */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 20px;
  margin-bottom: 28px;
}

.kpi-card {
  background: rgba(15, 23, 42, 0.8);
  border-radius: 16px;
  padding: 22px;
  position: relative;
  overflow: hidden;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  backdrop-filter: blur(8px);
}

.kpi-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.4);
}

.card-cyan {
  border: 1px solid rgba(6, 182, 212, 0.25);
  box-shadow: 0 4px 20px rgba(6, 182, 212, 0.08);
}

.card-blue {
  border: 1px solid rgba(59, 130, 246, 0.25);
  box-shadow: 0 4px 20px rgba(59, 130, 246, 0.08);
}

.card-violet {
  border: 1px solid rgba(139, 92, 246, 0.25);
  box-shadow: 0 4px 20px rgba(139, 92, 246, 0.08);
}

.card-emerald {
  border: 1px solid rgba(16, 185, 129, 0.25);
  box-shadow: 0 4px 20px rgba(16, 185, 129, 0.08);
}

.kpi-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.kpi-title {
  font-size: 13px;
  font-weight: 600;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.kpi-chip {
  font-size: 11px;
  font-weight: 600;
  padding: 2px 8px;
  border-radius: 12px;
  background: rgba(6, 182, 212, 0.15);
  color: #38bdf8;
}

.kpi-chip.blue {
  background: rgba(59, 130, 246, 0.15);
  color: #60a5fa;
}

.kpi-chip.violet {
  background: rgba(139, 92, 246, 0.15);
  color: #c084fc;
}

.kpi-chip.emerald {
  background: rgba(16, 185, 129, 0.15);
  color: #34d399;
}

.kpi-metric {
  font-size: 32px;
  font-weight: 800;
  color: #f8fafc;
  line-height: 1.1;
  margin-bottom: 12px;
}

.kpi-metric-unit {
  font-size: 14px;
  font-weight: 500;
  color: #64748b;
  margin-left: 4px;
}

.kpi-metric-sub {
  font-size: 14px;
  color: #94a3b8;
  font-weight: 500;
}

.text-cyan { color: #38bdf8; }
.text-emerald { color: #34d399; }

.kpi-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid rgba(255, 255, 255, 0.06);
  padding-top: 10px;
  font-size: 12px;
}

.footer-label {
  color: #64748b;
}

.footer-val {
  font-weight: 600;
  color: #e2e8f0;
}

/* Middle Section */
.content-row {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
  margin-bottom: 28px;
}

@media (max-width: 1024px) {
  .content-row {
    grid-template-columns: 1fr;
  }
}

.chart-panel, .channel-panel, .feed-card {
  background: rgba(15, 23, 42, 0.85);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 16px;
  padding: 24px;
  backdrop-filter: blur(8px);
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 20px;
}

.panel-title-group {
  display: flex;
  flex-direction: column;
}

.panel-title {
  font-size: 16px;
  font-weight: 700;
  color: #f8fafc;
}

.panel-subtitle {
  font-size: 12px;
  color: #64748b;
  margin-top: 2px;
}

.live-tag {
  display: flex;
  align-items: center;
  gap: 6px;
  background: rgba(56, 189, 248, 0.1);
  border: 1px solid rgba(56, 189, 248, 0.25);
  color: #38bdf8;
  font-size: 11px;
  font-weight: 600;
  padding: 4px 10px;
  border-radius: 20px;
}

.ping-dot {
  width: 6px;
  height: 6px;
  background: #38bdf8;
  border-radius: 50%;
}

.trend-chart-svg {
  width: 100%;
  height: 220px;
}

.chart-legend {
  display: flex;
  justify-content: center;
  gap: 24px;
  margin-top: 14px;
}

.legend-badge {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  color: #94a3b8;
}

.legend-color {
  width: 10px;
  height: 10px;
  border-radius: 3px;
}

.bg-cyan { background: #06b6d4; }
.bg-violet { background: #818cf8; }
.bg-emerald { background: #10b981; }

/* Channel Panel */
.channels-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.channel-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(30, 41, 59, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.05);
  padding: 12px 14px;
  border-radius: 12px;
}

.channel-main {
  display: flex;
  align-items: center;
  gap: 12px;
}

.channel-badge {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
}

.icon-search {
  background: rgba(6, 182, 212, 0.15);
  color: #06b6d4;
}

.icon-display {
  background: rgba(59, 130, 246, 0.15);
  color: #3b82f6;
}

.icon-social {
  background: rgba(139, 92, 246, 0.15);
  color: #8b5cf6;
}

.icon-video {
  background: rgba(16, 185, 129, 0.15);
  color: #10b981;
}

.channel-info {
  display: flex;
  flex-direction: column;
}

.channel-name {
  font-size: 13px;
  font-weight: 600;
  color: #f1f5f9;
}

.channel-meta {
  font-size: 11px;
  color: #64748b;
}

.channel-status {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  font-weight: 600;
}

.mini-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
}

.mini-dot.green { background: #10b981; }
.mini-dot.cyan { background: #06b6d4; }
.mini-dot.violet { background: #8b5cf6; }

.channel-footer-box {
  margin-top: 16px;
  background: rgba(6, 182, 212, 0.05);
  border: 1px dashed rgba(6, 182, 212, 0.25);
  border-radius: 10px;
  padding: 12px;
}

.footer-box-title {
  font-size: 12px;
  font-weight: 700;
  color: #38bdf8;
  margin-bottom: 2px;
}

.footer-box-desc {
  font-size: 11px;
  color: #94a3b8;
  line-height: 1.4;
}

/* Feed Table */
.feed-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 18px;
}

.feed-title {
  font-size: 16px;
  font-weight: 700;
  color: #f8fafc;
}

.feed-count {
  font-size: 12px;
  color: #64748b;
  margin-left: 10px;
}

.time-col {
  color: #94a3b8;
  font-size: 12px;
}

.event-pill {
  font-size: 11px;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: 12px;
  display: inline-block;
}

.pill-cyan {
  background: rgba(6, 182, 212, 0.15);
  color: #38bdf8;
}

.pill-violet {
  background: rgba(139, 92, 246, 0.15);
  color: #c084fc;
}

.pill-emerald {
  background: rgba(16, 185, 129, 0.15);
  color: #34d399;
}

.user-id-code {
  font-family: monospace;
  font-size: 12px;
  color: #cbd5e1;
  background: rgba(255, 255, 255, 0.05);
  padding: 2px 6px;
  border-radius: 4px;
}

.value-highlight {
  font-weight: 700;
  color: #34d399;
}

.text-subtle {
  color: #475569;
}

/* Dark Table Overrides */
:deep(.el-table) {
  background-color: transparent !important;
  color: #cbd5e1 !important;
}

:deep(.el-table tr) {
  background-color: transparent !important;
}

:deep(.el-table th.el-table__cell) {
  background-color: rgba(30, 41, 59, 0.5) !important;
  color: #94a3b8 !important;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08) !important;
  font-weight: 600;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

:deep(.el-table td.el-table__cell) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.05) !important;
}

:deep(.el-table--striped .el-table__body tr.el-table__row--striped td.el-table__cell) {
  background-color: rgba(255, 255, 255, 0.02) !important;
}

:deep(.el-table__body tr:hover > td.el-table__cell) {
  background-color: rgba(255, 255, 255, 0.05) !important;
}
</style>
