<template>
  <div class="events-view">
    <div class="page-header">
      <div class="header-intro">
        <div class="badge-row">
          <span class="live-dot"></span>
          <span class="badge-text">Real-Time Signal Exchange</span>
        </div>
        <h1 class="page-title">Live Ad Event Stream</h1>
        <p class="subtitle">Inspect live impression, engagement, click-through, and attribution events as they occur across networks</p>
      </div>

      <div class="header-actions">
        <el-select v-model="simCount" style="width: 130px; margin-right: 10px;" class="dark-select">
          <el-option :value="20" label="20 Signals" />
          <el-option :value="50" label="50 Signals" />
          <el-option :value="100" label="100 Signals" />
        </el-select>
        <el-button
          type="primary"
          class="sim-action-btn"
          icon="Promotion"
          :loading="simulating"
          @click="handleSimulate"
        >
          Inject Live Signals
        </el-button>
        <el-button class="refresh-btn" icon="Refresh" circle @click="loadEvents" :loading="loading" />
      </div>
    </div>

    <!-- Product Pipeline Status Highlights -->
    <div class="kpi-grid">
      <div class="info-card border-cyan">
        <div class="info-num text-cyan">Sub-Millisecond Pipeline</div>
        <div class="info-lbl">Edge Signal Ingestion & Low-Latency Buffering</div>
      </div>
      <div class="info-card border-violet">
        <div class="info-num text-violet">Attribution Verification</div>
        <div class="info-lbl">Deduplication & Multi-Touch Attribution Engine</div>
      </div>
      <div class="info-card border-emerald">
        <div class="info-num text-emerald">Tenant Integrity Guard</div>
        <div class="info-lbl">Cryptographic Multi-Tenant Context Isolation</div>
      </div>
    </div>

    <!-- Events Table Card -->
    <div class="table-container">
      <div class="table-header">
        <span class="table-title">Captured Event Telemetry</span>
        <span class="table-count">{{ events.length }} events in active window</span>
      </div>

      <el-table :data="events" stripe class="dark-table" v-loading="loading">
        <el-table-column prop="eventTime" label="Timestamp" width="180">
          <template #default="{ row }">
            <span class="time-text">{{ formatTime(row.eventTime) }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="eventType" label="Event Type" width="140">
          <template #default="{ row }">
            <span class="event-pill" :class="getEventTypeClass(row.eventType)">
              {{ getEventTypeName(row.eventType) }}
            </span>
          </template>
        </el-table-column>

        <el-table-column prop="id" label="Event ID" min-width="180">
          <template #default="{ row }">
            <span class="event-guid">{{ row.id }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="userId" label="Audience ID" width="150">
          <template #default="{ row }">
            <span class="audience-id">{{ row.userId }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="deviceType" label="Device" width="120" />

        <el-table-column prop="country" label="Geography" width="160">
          <template #default="{ row }">
            <span>{{ row.city ? `${row.city}, ` : '' }}{{ row.country }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="conversionValue" label="Attributed Value" width="150">
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
import { eventsApi } from '../api/events';
import type { EventDto } from '../types';
import { formatTime } from '../utils/date';

const loading = ref(false);
const simulating = ref(false);
const simCount = ref(50);
const events = ref<EventDto[]>([]);

async function loadEvents() {
  loading.value = true;
  try {
    events.value = await eventsApi.getRecentEvents(undefined, 50);
  } catch (error) {
    console.error('Failed to load events:', error);
  } finally {
    loading.value = false;
  }
}

async function handleSimulate() {
  simulating.value = true;
  try {
    const res = await eventsApi.simulateEvents(simCount.value);
    ElMessage.success(res.message || `Injected ${simCount.value} ad signals successfully.`);
    await loadEvents();
  } catch (err: any) {
    ElMessage.error(err.message || 'Signal injection failed');
  } finally {
    simulating.value = false;
  }
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
  loadEvents();
});
</script>

<style scoped>
.events-view {
  padding: 32px 36px;
  max-width: 1540px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 24px;
  flex-wrap: wrap;
  gap: 16px;
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

.live-dot {
  width: 6px;
  height: 6px;
  background: #38bdf8;
  border-radius: 50%;
  box-shadow: 0 0 8px #38bdf8;
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

.header-actions {
  display: flex;
  align-items: center;
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

/* KPI Grid */
.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}

.info-card {
  background: rgba(15, 23, 42, 0.75);
  border-radius: 12px;
  padding: 18px 20px;
  backdrop-filter: blur(8px);
}

.border-cyan { border: 1px solid rgba(6, 182, 212, 0.25); }
.border-violet { border: 1px solid rgba(139, 92, 246, 0.25); }
.border-emerald { border: 1px solid rgba(16, 185, 129, 0.25); }

.info-num {
  font-size: 16px;
  font-weight: 700;
  margin-bottom: 4px;
}

.info-lbl {
  font-size: 12px;
  color: #94a3b8;
}

.text-cyan { color: #38bdf8; }
.text-violet { color: #c084fc; }
.text-emerald { color: #34d399; }

/* Table */
.table-container {
  background: rgba(15, 23, 42, 0.8);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 16px;
  padding: 24px;
  backdrop-filter: blur(8px);
}

.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 18px;
}

.table-title {
  font-size: 16px;
  font-weight: 700;
  color: #f8fafc;
}

.table-count {
  font-size: 12px;
  color: #64748b;
}

.time-text {
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

.event-guid {
  font-family: monospace;
  font-size: 11px;
  color: #64748b;
}

.audience-id {
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

/* Table overrides */
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
