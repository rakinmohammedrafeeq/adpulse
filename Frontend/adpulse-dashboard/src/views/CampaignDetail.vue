<template>
  <div class="campaign-detail-view" v-loading="loading">
    <div v-if="campaign" class="detail-content">
      <!-- Header Banner -->
      <div class="detail-header">
        <div class="title-area">
          <div class="header-breadcrumbs">
            <router-link to="/campaigns" class="crumb-link">&larr; All Campaigns</router-link>
          </div>
          <h2>{{ campaign.name }}</h2>
          <div class="header-badges">
            <el-tag :type="CampaignStatusTagType[campaign.status] || 'info'" effect="dark">
              {{ CampaignStatusLabels[campaign.status] }}
            </el-tag>
            <el-tag type="info" effect="plain">
              Objective: {{ CampaignObjectiveLabels[campaign.objective] }}
            </el-tag>
            <span class="text-muted text-sm">
              Schedule: {{ formatDate(campaign.startDate) }} &rarr; {{ campaign.endDate ? formatDate(campaign.endDate) : 'Ongoing' }}
            </span>
          </div>
        </div>

        <div class="header-actions">
          <el-button type="primary" icon="Edit" @click="$router.push(`/campaigns/${campaign.id}/edit`)">
            Edit Campaign
          </el-button>
          <el-button type="success" icon="Promotion" @click="handleSimulateForCampaign" :loading="simulating">
            Simulate Traffic Here
          </el-button>
        </div>
      </div>

      <!-- Quick Metrics Ribbon -->
      <el-row :gutter="16" class="ribbon-grid">
        <el-col :span="6">
          <el-card shadow="hover" class="ribbon-card">
            <div class="ribbon-label">Daily Budget</div>
            <div class="ribbon-val text-primary">${{ Number(campaign.dailyBudget).toFixed(2) }}</div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card shadow="hover" class="ribbon-card">
            <div class="ribbon-label">Total Spend</div>
            <div class="ribbon-val text-warning">${{ Number(campaign.spentAmount).toFixed(2) }}</div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card shadow="hover" class="ribbon-card">
            <div class="ribbon-label">Lifetime Budget</div>
            <div class="ribbon-val text-info">${{ campaign.totalBudget ? Number(campaign.totalBudget).toFixed(2) : 'Flexible' }}</div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card shadow="hover" class="ribbon-card">
            <div class="ribbon-label">Ad Groups</div>
            <div class="ribbon-val text-success">{{ campaign.adGroups?.length || 0 }}</div>
          </el-card>
        </el-col>
      </el-row>

      <!-- Main Tabs -->
      <el-card shadow="never" class="tabs-card">
        <el-tabs v-model="activeTab">
          <!-- TAB 1: AD GROUPS & CREATIVES -->
          <el-tab-pane label="Ad Groups & Creatives" name="adgroups">
            <div class="tab-action-bar">
              <h4>Ad Groups in this Campaign</h4>
              <el-button type="primary" size="small" icon="Plus" @click="showAdGroupModal = true">
                Add Ad Group
              </el-button>
            </div>

            <div v-if="campaign.adGroups?.length === 0" class="empty-state">
              <el-empty description="No ad groups created yet. Add an ad group to start serving creatives." />
            </div>

            <div v-else class="adgroups-stack">
              <el-collapse v-model="activeCollapse">
                <el-collapse-item
                  v-for="ag in campaign.adGroups"
                  :key="ag.id"
                  :title="`${ag.name} (Bid: $${Number(ag.bidAmount).toFixed(2)})`"
                  :name="ag.id"
                >
                  <div class="adgroup-body">
                    <div class="adgroup-meta">
                      <el-tag size="small">{{ ag.biddingStrategy === 0 ? 'Manual CPC' : 'Auto Bidding' }}</el-tag>
                      <span class="text-muted text-xs">Created: {{ formatDate(ag.createdAt) }}</span>
                      <el-button type="primary" link size="small" icon="Plus" @click="openCreateCreativeModal(ag.id)">
                        Add Creative
                      </el-button>
                    </div>

                    <!-- Creatives in this group -->
                    <div class="creatives-gallery" v-if="adGroupCreatives[ag.id]?.length">
                      <el-card
                        v-for="cr in adGroupCreatives[ag.id]"
                        :key="cr.id"
                        class="creative-card"
                        shadow="hover"
                      >
                        <div v-if="cr.imageUrl" class="creative-img-wrapper">
                          <img :src="cr.imageUrl" alt="Creative Preview" class="creative-img" />
                        </div>
                        <div class="creative-card-body">
                          <div class="cr-title">{{ cr.name }}</div>
                          <div class="cr-headline">{{ cr.headline }}</div>
                          <el-tag size="small" type="info" class="cr-tag">{{ cr.type === 0 ? 'Image Ad' : 'Responsive' }}</el-tag>
                        </div>
                      </el-card>
                    </div>
                    <div v-else class="text-muted text-sm" style="padding: 12px;">
                      No creatives in this ad group yet.
                    </div>
                  </div>
                </el-collapse-item>
              </el-collapse>
            </div>
          </el-tab-pane>

          <!-- TAB 2: PERFORMANCE METRICS -->
          <el-tab-pane label="Performance & Events" name="metrics">
            <h4>Live Ingested Traffic for {{ campaign.name }}</h4>
            <el-table :data="campaignEvents" stripe style="width: 100%" v-loading="loadingEvents">
              <el-table-column prop="eventTime" label="Timestamp" width="180">
                <template #default="{ row }">
                  {{ formatTime(row.eventTime) }}
                </template>
              </el-table-column>
              <el-table-column prop="eventType" label="Event Type" width="140">
                <template #default="{ row }">
                  <el-tag :type="row.eventType === 1 ? 'warning' : row.eventType === 2 ? 'success' : 'info'" size="small">
                    {{ row.eventType === 0 ? 'Impression' : row.eventType === 1 ? 'Click' : row.eventType === 2 ? 'Conversion' : 'Video' }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column prop="userId" label="User ID" width="160" />
              <el-table-column prop="deviceType" label="Device" width="130" />
              <el-table-column prop="country" label="Country" width="150" />
              <el-table-column prop="conversionValue" label="Revenue">
                <template #default="{ row }">
                  <span v-if="row.conversionValue" class="text-success font-bold">${{ Number(row.conversionValue).toFixed(2) }}</span>
                  <span v-else class="text-muted">-</span>
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
        </el-tabs>
      </el-card>

      <!-- DIALOG: CREATE AD GROUP -->
      <el-dialog v-model="showAdGroupModal" title="Create New Ad Group" width="500px">
        <el-form :model="newAdGroup" label-position="top">
          <el-form-item label="Ad Group Name" required>
            <el-input v-model="newAdGroup.name" placeholder="e.g. US Display Audience" />
          </el-form-item>
          <el-form-item label="Bidding Strategy">
            <el-select v-model="newAdGroup.biddingStrategy" style="width: 100%;">
              <el-option :value="0" label="Manual CPC" />
              <el-option :value="1" label="Manual CPM" />
              <el-option :value="2" label="Automatic CPC" />
              <el-option :value="3" label="Target CPA" />
            </el-select>
          </el-form-item>
          <el-form-item label="Target Bid Amount ($ USD)" required>
            <el-input-number v-model="newAdGroup.bidAmount" :min="0.10" :step="0.25" style="width: 100%;" />
          </el-form-item>
        </el-form>
        <template #footer>
          <el-button @click="showAdGroupModal = false">Cancel</el-button>
          <el-button type="primary" :loading="savingAdGroup" @click="handleCreateAdGroup">Create Ad Group</el-button>
        </template>
      </el-dialog>

      <!-- DIALOG: CREATE CREATIVE -->
      <el-dialog v-model="showCreativeModal" title="Create Creative" width="550px">
        <el-form :model="newCreative" label-position="top">
          <el-form-item label="Creative Name" required>
            <el-input v-model="newCreative.name" placeholder="e.g. Summer Promo Banner 1200x628" />
          </el-form-item>
          <el-form-item label="Headline" required>
            <el-input v-model="newCreative.headline" placeholder="Compelling call to action" />
          </el-form-item>
          <el-form-item label="Image URL">
            <el-input v-model="newCreative.imageUrl" placeholder="https://..." />
          </el-form-item>
          <el-form-item label="Destination Landing Page URL" required>
            <el-input v-model="newCreative.destinationUrl" placeholder="https://example.com/landing" />
          </el-form-item>
        </el-form>
        <template #footer>
          <el-button @click="showCreativeModal = false">Cancel</el-button>
          <el-button type="primary" :loading="savingCreative" @click="handleCreateCreative">Create Creative</el-button>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { ElMessage } from 'element-plus';
import { campaignsApi } from '../api/campaigns';
import { adGroupsApi } from '../api/adGroups';
import { creativesApi } from '../api/creatives';
import { eventsApi } from '../api/events';
import { CampaignStatusLabels, CampaignStatusTagType, CampaignObjectiveLabels } from '../types';
import type { CampaignDetailDto, EventDto, CreativeListDto } from '../types';
import { formatDate, formatTime } from '../utils/date';

const route = useRoute();
const campaignId = route.params.id as string;

const loading = ref(false);
const loadingEvents = ref(false);
const simulating = ref(false);
const campaign = ref<CampaignDetailDto | null>(null);
const activeTab = ref('adgroups');
const activeCollapse = ref<string[]>([]);
const campaignEvents = ref<EventDto[]>([]);
const adGroupCreatives = reactive<Record<string, CreativeListDto[]>>({});

// Modal states
const showAdGroupModal = ref(false);
const savingAdGroup = ref(false);
const newAdGroup = reactive({
  name: '',
  biddingStrategy: 2,
  bidAmount: 1.50
});

const showCreativeModal = ref(false);
const activeTargetAdGroupId = ref('');
const savingCreative = ref(false);
const newCreative = reactive({
  name: '',
  headline: '',
  imageUrl: 'https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=600&auto=format&fit=crop&q=60',
  destinationUrl: 'https://adpulse.example.com/promo'
});

async function loadCampaign() {
  loading.value = true;
  try {
    const data = await campaignsApi.getCampaign(campaignId);
    campaign.value = data;
    if (data.adGroups?.length) {
      activeCollapse.value = [data.adGroups[0].id];
      // Load creatives for each adgroup
      for (const ag of data.adGroups) {
        try {
          const crs = await creativesApi.getCreatives(ag.id);
          adGroupCreatives[ag.id] = crs;
        } catch (e) {
          adGroupCreatives[ag.id] = [];
        }
      }
    }
  } catch (err: any) {
    ElMessage.error('Failed to load campaign');
  } finally {
    loading.value = false;
  }
}

async function loadEvents() {
  loadingEvents.value = true;
  try {
    campaignEvents.value = await eventsApi.getRecentEvents(campaignId, 25);
  } finally {
    loadingEvents.value = false;
  }
}

async function handleSimulateForCampaign() {
  simulating.value = true;
  try {
    const res = await eventsApi.simulateEvents(25, undefined, campaignId);
    ElMessage.success(res.message || 'Generated 25 events for this campaign!');
    await loadCampaign();
    await loadEvents();
  } catch (err: any) {
    ElMessage.error(err.message || 'Simulation failed');
  } finally {
    simulating.value = false;
  }
}

async function handleCreateAdGroup() {
  if (!newAdGroup.name) return;
  savingAdGroup.value = true;
  try {
    await adGroupsApi.createAdGroup(campaignId, {
      name: newAdGroup.name,
      biddingStrategy: newAdGroup.biddingStrategy,
      bidAmount: newAdGroup.bidAmount
    });
    ElMessage.success('Ad group created!');
    showAdGroupModal.value = false;
    newAdGroup.name = '';
    await loadCampaign();
  } catch (err: any) {
    ElMessage.error(err.message || 'Failed to create ad group');
  } finally {
    savingAdGroup.value = false;
  }
}

function openCreateCreativeModal(adGroupId: string) {
  activeTargetAdGroupId.value = adGroupId;
  showCreativeModal.value = true;
}

async function handleCreateCreative() {
  if (!newCreative.name || !newCreative.headline) return;
  savingCreative.value = true;
  try {
    await creativesApi.createCreative(activeTargetAdGroupId.value, {
      name: newCreative.name,
      type: 0,
      headline: newCreative.headline,
      imageUrl: newCreative.imageUrl,
      destinationUrl: newCreative.destinationUrl
    });
    ElMessage.success('Creative added!');
    showCreativeModal.value = false;
    newCreative.name = '';
    newCreative.headline = '';
    await loadCampaign();
  } catch (err: any) {
    ElMessage.error(err.message || 'Failed to create creative');
  } finally {
    savingCreative.value = false;
  }
}

onMounted(() => {
  loadCampaign();
  loadEvents();
});
</script>

<style scoped>
.campaign-detail-view {
  padding: 24px;
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 20px;
  flex-wrap: wrap;
  gap: 16px;
}

.header-breadcrumbs {
  margin-bottom: 8px;
}

.crumb-link {
  color: #38bdf8;
  text-decoration: none;
  font-size: 13px;
  font-weight: 500;
  transition: color 0.2s;
}

.crumb-link:hover {
  color: #7dd3fc;
}

.title-area h2 {
  margin: 0 0 8px;
  font-size: 24px;
  font-weight: 800;
  color: #f8fafc;
  letter-spacing: -0.02em;
}

.header-badges {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.ribbon-grid {
  margin-bottom: 16px;
}

.ribbon-card {
  border-radius: 12px;
  text-align: center;
}

.ribbon-label {
  font-size: 11px;
  text-transform: uppercase;
  color: #94a3b8;
  margin-bottom: 4px;
  letter-spacing: 0.5px;
  font-weight: 600;
}

.ribbon-val {
  font-size: 22px;
  font-weight: 700;
}

.tabs-card {
  border-radius: 16px;
}

.tab-action-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.tab-action-bar h4 {
  margin: 0;
  color: #f1f5f9;
  font-size: 16px;
  font-weight: 700;
}

.adgroups-stack {
  margin-top: 10px;
}

.adgroup-meta {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
}

.creatives-gallery {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 14px;
}

.creative-card {
  border-radius: 12px;
  overflow: hidden;
  background: rgba(30, 41, 59, 0.4) !important;
  border: 1px solid rgba(255, 255, 255, 0.08) !important;
}

.creative-img-wrapper {
  height: 140px;
  overflow: hidden;
  background: rgba(15, 23, 42, 0.8);
}

.creative-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.creative-card-body {
  padding: 12px;
}

.cr-title {
  font-weight: 600;
  font-size: 14px;
  color: #f1f5f9;
  margin-bottom: 4px;
}

.cr-headline {
  font-size: 12px;
  color: #94a3b8;
  margin-bottom: 6px;
}

.text-primary { color: #38bdf8; }
.text-warning { color: #fbbf24; }
.text-info { color: #818cf8; }
.text-success { color: #34d399; }
.text-muted { color: #94a3b8; }
.text-sm { font-size: 12px; }
.text-xs { font-size: 11px; }
.font-bold { font-weight: 600; }
</style>
