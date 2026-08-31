<template>
  <div class="campaigns-view">
    <div class="page-header">
      <div>
        <h2>Campaign Management</h2>
        <p class="subtitle">Search, manage, and optimize multi-channel advertising campaigns</p>
      </div>

      <el-button type="primary" icon="Plus" @click="$router.push('/campaigns/create')">
        Create Campaign
      </el-button>
    </div>

    <!-- Search & Filter Controls -->
    <el-card shadow="never" class="filter-card">
      <el-row :gutter="16">
        <el-col :xs="24" :sm="12" :md="10">
          <el-input
            v-model="searchQuery"
            placeholder="Search campaigns by name, objective, or target keywords..."
            clearable
            prefix-icon="Search"
            @input="handleSearchInput"
          >
            <template #append>
              <el-button icon="Search" @click="handleSearch">Search</el-button>
            </template>
          </el-input>
        </el-col>

        <el-col :xs="12" :sm="6" :md="6">
          <el-select v-model="selectedStatus" placeholder="Filter by Status" clearable @change="handleFilterChange" style="width: 100%;">
            <el-option :value="-1" label="All Statuses" />
            <el-option :value="2" label="Active" />
            <el-option :value="3" label="Paused" />
            <el-option :value="0" label="Draft" />
            <el-option :value="4" label="Completed" />
          </el-select>
        </el-col>

        <el-col :xs="12" :sm="6" :md="8" class="text-right">
          <el-button icon="Refresh" @click="loadCampaigns">Refresh</el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- Campaigns Table -->
    <el-card shadow="hover" class="table-card">
      <el-table :data="filteredCampaigns" stripe style="width: 100%" v-loading="loading">
        <el-table-column prop="name" label="Campaign Name" min-width="200">
          <template #default="{ row }">
            <router-link :to="`/campaigns/${row.id}`" class="campaign-link font-bold">
              {{ row.name }}
            </router-link>
            <div class="text-muted text-xs">{{ row.id }}</div>
          </template>
        </el-table-column>

        <el-table-column prop="status" label="Status" width="130">
          <template #default="{ row }">
            <el-tag :type="CampaignStatusTagType[row.status] || 'info'" size="small">
              {{ CampaignStatusLabels[row.status] || 'Unknown' }}
            </el-tag>
          </template>
        </el-table-column>

        <el-table-column prop="objective" label="Objective" width="160">
          <template #default="{ row }">
            {{ CampaignObjectiveLabels[row.objective] || 'General' }}
          </template>
        </el-table-column>

        <el-table-column prop="dailyBudget" label="Daily Budget" width="140">
          <template #default="{ row }">
            ${{ Number(row.dailyBudget).toFixed(2) }}
          </template>
        </el-table-column>

        <el-table-column prop="spentAmount" label="Spent Amount" width="140">
          <template #default="{ row }">
            <span class="font-bold">${{ Number(row.spentAmount).toFixed(2) }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="startDate" label="Schedule" min-width="180">
          <template #default="{ row }">
            <span>{{ formatDate(row.startDate) }} &rarr; {{ row.endDate ? formatDate(row.endDate) : 'Ongoing' }}</span>
          </template>
        </el-table-column>

        <el-table-column label="Actions" width="180" fixed="right">
          <template #default="{ row }">
            <el-button-group size="small">
              <el-button icon="View" @click="$router.push(`/campaigns/${row.id}`)" />
              <el-button icon="Edit" @click="$router.push(`/campaigns/${row.id}/edit`)" />
              <el-button
                :type="row.status === 2 ? 'warning' : 'success'"
                :icon="row.status === 2 ? 'VideoPause' : 'VideoPlay'"
                @click="toggleCampaignStatus(row)"
              />
              <el-button type="danger" icon="Delete" @click="promptDelete(row)" />
            </el-button-group>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Delete Confirmation Modal -->
    <el-dialog
      v-model="deleteDialogVisible"
      title="Delete Campaign"
      width="420px"
      align-center
      :close-on-click-modal="false"
    >
      <div class="confirm-body">
        <div class="confirm-icon confirm-icon--danger">
          <el-icon><Delete /></el-icon>
        </div>
        <p class="confirm-text">Delete <strong>{{ pendingDeleteRow?.name }}</strong>?</p>
        <p class="confirm-sub">This action cannot be undone. All associated ad groups, creatives, and events will be permanently removed.</p>
      </div>
      <template #footer>
        <el-button @click="deleteDialogVisible = false">Cancel</el-button>
        <el-button type="danger" :loading="deleting" @click="confirmDelete">Yes, Delete Campaign</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { Delete } from '@element-plus/icons-vue';
import { campaignsApi } from '../api/campaigns';
import { analyticsApi } from '../api/analytics';
import { CampaignStatusLabels, CampaignStatusTagType, CampaignObjectiveLabels } from '../types';
import type { CampaignListDto } from '../types';
import { formatDate } from '../utils/date';

const loading = ref(false);
const campaigns = ref<CampaignListDto[]>([]);
const searchQuery = ref('');
const selectedStatus = ref<number>(-1);

// Delete modal state
const deleteDialogVisible = ref(false);
const deleting = ref(false);
const pendingDeleteRow = ref<CampaignListDto | null>(null);

function promptDelete(row: CampaignListDto) {
  pendingDeleteRow.value = row;
  deleteDialogVisible.value = true;
}

async function confirmDelete() {
  if (!pendingDeleteRow.value) return;
  deleting.value = true;
  try {
    await campaignsApi.deleteCampaign(pendingDeleteRow.value.id);
    campaigns.value = campaigns.value.filter((c) => c.id !== pendingDeleteRow.value!.id);
    ElMessage.success('Campaign deleted successfully.');
    deleteDialogVisible.value = false;
  } catch (err: any) {
    ElMessage.error(err.message || 'Delete failed');
  } finally {
    deleting.value = false;
    pendingDeleteRow.value = null;
  }
}

async function loadCampaigns() {
  loading.value = true;
  try {
    campaigns.value = await campaignsApi.getCampaigns();
  } catch (error) {
    console.error('Failed to load campaigns:', error);
  } finally {
    loading.value = false;
  }
}

async function handleSearch() {
  if (!searchQuery.value.trim()) {
    return loadCampaigns();
  }

  loading.value = true;
  try {
    const res = await analyticsApi.searchCampaigns(searchQuery.value.trim());
    if (res && res.results) {
      campaigns.value = res.results.map((r: any) => ({
        id: r.id,
        name: r.name,
        status: r.status,
        objective: r.objective,
        dailyBudget: r.dailyBudget,
        spentAmount: r.spentAmount,
        startDate: r.startDate,
        endDate: r.endDate,
        createdAt: r.createdAt
      }));
      ElMessage.success(`Elasticsearch found ${res.results.length} campaigns matching query.`);
    }
  } catch (error) {
    console.warn('Elasticsearch search fallback to local filter:', error);
  } finally {
    loading.value = false;
  }
}

function handleSearchInput() {
  if (searchQuery.value.trim() === '') {
    loadCampaigns();
  }
}

function handleFilterChange() {
  // handled by computed
}

const filteredCampaigns = computed(() => {
  return campaigns.value.filter((c) => {
    const matchStatus = selectedStatus.value === -1 || c.status === selectedStatus.value;
    const matchSearch =
      !searchQuery.value ||
      c.name.toLowerCase().includes(searchQuery.value.toLowerCase());
    return matchStatus && matchSearch;
  });
});

async function toggleCampaignStatus(campaign: CampaignListDto) {
  const newStatus = campaign.status === 2 ? 3 : 2; // 2 = Active, 3 = Paused
  try {
    await campaignsApi.updateCampaign(campaign.id, { status: newStatus });
    campaign.status = newStatus;
    ElMessage.success(`Campaign ${newStatus === 2 ? 'activated' : 'paused'} successfully.`);
  } catch (err: any) {
    ElMessage.error(err.message || 'Status update failed');
  }
}

onMounted(() => {
  loadCampaigns();
});
</script>

<style scoped>
.campaigns-view {
  padding: 32px 36px;
  max-width: 1540px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-header h2 {
  margin: 0 0 6px;
  font-size: 26px;
  font-weight: 800;
  color: #f8fafc;
  letter-spacing: -0.5px;
}

.subtitle {
  margin: 0;
  font-size: 14px;
  color: #94a3b8;
}

.filter-card {
  margin-bottom: 20px;
  background: rgba(15, 23, 42, 0.75) !important;
  border: 1px solid rgba(255, 255, 255, 0.08) !important;
  border-radius: 14px !important;
}

.table-card {
  border-radius: 16px !important;
  background: rgba(15, 23, 42, 0.85) !important;
  border: 1px solid rgba(255, 255, 255, 0.08) !important;
}

.campaign-link {
  color: #38bdf8;
  text-decoration: none;
  font-size: 14px;
}

.campaign-link:hover {
  text-decoration: underline;
  color: #7dd3fc;
}

.text-right {
  text-align: right;
}

.text-muted {
  color: #64748b;
}

.text-xs {
  font-size: 11px;
  font-family: monospace;
  margin-top: 2px;
}

.font-bold {
  font-weight: 600;
}
</style>
