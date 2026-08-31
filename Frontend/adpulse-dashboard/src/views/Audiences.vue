<template>
  <div class="audiences-view">
    <div class="page-header">
      <div>
        <h2>Audience & Targeting Configuration</h2>
        <p class="subtitle">Segment user cohorts, configure lookalikes, and define targeting parameters</p>
      </div>

      <el-button type="primary" icon="Plus" @click="showCreateModal = true">
        Create Audience Segment
      </el-button>
    </div>

    <!-- Audiences Table -->
    <el-card shadow="hover" class="table-card">
      <el-table :data="audiences" stripe style="width: 100%" v-loading="loading">
        <el-table-column prop="name" label="Segment Name" min-width="180">
          <template #default="{ row }">
            <div class="font-bold">{{ row.name }}</div>
            <div class="text-muted text-xs">{{ row.description || 'No description provided' }}</div>
          </template>
        </el-table-column>

        <el-table-column prop="type" label="Type" width="140">
          <template #default="{ row }">
            <el-tag :type="row.type === 1 ? 'warning' : row.type === 2 ? 'danger' : 'primary'" size="small">
              {{ row.type === 0 ? 'Custom Segment' : row.type === 1 ? 'Lookalike' : row.type === 2 ? 'Retargeting' : 'Demographics' }}
            </el-tag>
          </template>
        </el-table-column>

        <el-table-column prop="estimatedSize" label="Audience Reach" width="160">
          <template #default="{ row }">
            <span class="font-bold text-primary">{{ Number(row.estimatedSize).toLocaleString() }}</span> users
          </template>
        </el-table-column>

        <el-table-column label="Targeting Criteria" min-width="220">
          <template #default="{ row }">
            <div class="criteria-tags">
              <el-tag v-if="row.locations" size="small" type="info">Geo: {{ row.locations }}</el-tag>
              <el-tag v-if="row.devices" size="small" type="info">Dev: {{ row.devices }}</el-tag>
              <el-tag v-if="row.interests" size="small" type="info">Int: {{ row.interests }}</el-tag>
            </div>
          </template>
        </el-table-column>

        <el-table-column prop="createdAt" label="Created" width="140">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>

        <el-table-column label="Actions" width="120" fixed="right">
          <template #default="{ row }">
            <el-button type="danger" icon="Delete" size="small" @click="promptDeleteAudience(row)" />
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- CREATE AUDIENCE DIALOG -->
    <el-dialog v-model="showCreateModal" title="Create Audience Segment" width="550px">
      <el-form :model="newAudience" label-position="top">
        <el-form-item label="Audience Name" required>
          <el-input v-model="newAudience.name" placeholder="e.g. North America Tech Executives" />
        </el-form-item>

        <el-form-item label="Description">
          <el-input v-model="newAudience.description" type="textarea" :rows="2" placeholder="Description of this target audience segment" />
        </el-form-item>

        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="Audience Type">
              <el-select v-model="newAudience.type" style="width: 100%;">
                <el-option :value="0" label="Custom Segment" />
                <el-option :value="1" label="Lookalike (AI-modeled)" />
                <el-option :value="2" label="Retargeting" />
                <el-option :value="3" label="Demographics" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Estimated Size" required>
              <el-input-number v-model="newAudience.estimatedSize" :min="1000" :step="50000" style="width: 100%;" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="Locations (Comma separated)">
          <el-input v-model="newAudience.locations" placeholder="e.g. United States, United Kingdom, Canada" />
        </el-form-item>

        <el-form-item label="Interests">
          <el-input v-model="newAudience.interests" placeholder="e.g. Cloud Computing, DevOps, Enterprise IT" />
        </el-form-item>

        <el-form-item label="Devices">
          <el-input v-model="newAudience.devices" placeholder="e.g. Desktop, Mobile" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="showCreateModal = false">Cancel</el-button>
        <el-button type="primary" :loading="saving" @click="handleCreateAudience">Save Segment</el-button>
      </template>
    </el-dialog>
    <!-- Delete Audience Confirmation Modal -->
    <el-dialog
      v-model="deleteAudienceVisible"
      title="Delete Audience Segment"
      width="420px"
      align-center
      :close-on-click-modal="false"
    >
      <div class="confirm-body">
        <div class="confirm-icon confirm-icon--danger">
          <el-icon><Delete /></el-icon>
        </div>
        <p class="confirm-text">Delete <strong>{{ pendingDeleteAudience?.name }}</strong>?</p>
        <p class="confirm-sub">This audience segment will be permanently removed and unlinked from all campaigns.</p>
      </div>
      <template #footer>
        <el-button @click="deleteAudienceVisible = false">Cancel</el-button>
        <el-button type="danger" :loading="deletingAudience" @click="confirmDeleteAudience">Yes, Delete</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { Delete } from '@element-plus/icons-vue';
import { audiencesApi } from '../api/audiences';
import type { AudienceDto } from '../types';
import { formatDate } from '../utils/date';

const loading = ref(false);
const saving = ref(false);
const audiences = ref<AudienceDto[]>([]);
const showCreateModal = ref(false);

// Delete modal state
const deleteAudienceVisible = ref(false);
const deletingAudience = ref(false);
const pendingDeleteAudience = ref<AudienceDto | null>(null);

function promptDeleteAudience(row: AudienceDto) {
  pendingDeleteAudience.value = row;
  deleteAudienceVisible.value = true;
}

async function confirmDeleteAudience() {
  if (!pendingDeleteAudience.value) return;
  deletingAudience.value = true;
  try {
    await audiencesApi.deleteAudience(pendingDeleteAudience.value.id);
    audiences.value = audiences.value.filter((a) => a.id !== pendingDeleteAudience.value!.id);
    ElMessage.success('Audience segment deleted');
    deleteAudienceVisible.value = false;
  } catch (err: any) {
    ElMessage.error(err.message || 'Failed to delete');
  } finally {
    deletingAudience.value = false;
    pendingDeleteAudience.value = null;
  }
}

const newAudience = reactive({
  name: '',
  description: '',
  type: 0,
  estimatedSize: 500000,
  locations: 'United States, United Kingdom',
  interests: 'Cloud Computing, SaaS, Marketing Automation',
  devices: 'Desktop, Mobile'
});

async function loadAudiences() {
  loading.value = true;
  try {
    audiences.value = await audiencesApi.getAudiences();
  } catch (error) {
    console.error('Failed to load audiences:', error);
  } finally {
    loading.value = false;
  }
}

async function handleCreateAudience() {
  if (!newAudience.name) return;
  saving.value = true;
  try {
    await audiencesApi.createAudience({
      name: newAudience.name,
      description: newAudience.description,
      type: newAudience.type,
      estimatedSize: newAudience.estimatedSize,
      locations: newAudience.locations,
      interests: newAudience.interests,
      devices: newAudience.devices
    });

    ElMessage.success('Audience segment created!');
    showCreateModal.value = false;
    newAudience.name = '';
    await loadAudiences();
  } catch (err: any) {
    ElMessage.error(err.message || 'Failed to create audience');
  } finally {
    saving.value = false;
  }
}

onMounted(() => {
  loadAudiences();
});
</script>

<style scoped>
.audiences-view {
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

.table-card {
  border-radius: 16px !important;
  background: rgba(15, 23, 42, 0.85) !important;
  border: 1px solid rgba(255, 255, 255, 0.08) !important;
}

.criteria-tags {
  display: flex;
  gap: 6px;
  flex-wrap: wrap;
}

.font-bold { font-weight: 600; }
.text-primary { color: #38bdf8; }
.text-muted { color: #64748b; }
.text-xs { font-size: 12px; }
</style>
