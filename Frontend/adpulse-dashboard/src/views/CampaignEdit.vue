<template>
  <div class="campaign-form-view">
    <div class="page-header">
      <div>
        <h2>Edit Campaign: {{ form.name }}</h2>
        <p class="subtitle">Modify budget allocations, delivery status, and scheduling</p>
      </div>

      <el-button @click="$router.push(`/campaigns/${campaignId}`)">Back to Campaign</el-button>
    </div>

    <el-card shadow="hover" class="form-card" v-loading="loading">
      <el-form :model="form" :rules="rules" ref="formRef" label-position="top">
        <el-row :gutter="20">
          <el-col :span="16">
            <el-form-item label="Campaign Name" prop="name">
              <el-input v-model="form.name" />
            </el-form-item>
          </el-col>

          <el-col :span="8">
            <el-form-item label="Delivery Status" prop="status">
              <el-select v-model="form.status" style="width: 100%;">
                <el-option :value="0" label="Draft" />
                <el-option :value="1" label="Scheduled" />
                <el-option :value="2" label="Active" />
                <el-option :value="3" label="Paused" />
                <el-option :value="4" label="Completed" />
                <el-option :value="5" label="Archived" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="Description" prop="description">
          <el-input v-model="form.description" type="textarea" :rows="3" />
        </el-form-item>

        <el-divider content-position="left">Budget Adjustments</el-divider>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Daily Budget ($ USD)" prop="dailyBudget">
              <el-input-number v-model="form.dailyBudget" :min="1" :step="25" style="width: 100%;" />
            </el-form-item>
          </el-col>

          <el-col :span="12">
            <el-form-item label="Total Lifetime Budget ($ USD)" prop="totalBudget">
              <el-input-number v-model="form.totalBudget" :min="1" :step="100" style="width: 100%;" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-divider content-position="left">Schedule</el-divider>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Start Date" prop="startDate">
              <el-date-picker v-model="form.startDate" type="datetime" style="width: 100%;" />
            </el-form-item>
          </el-col>

          <el-col :span="12">
            <el-form-item label="End Date (Optional)" prop="endDate">
              <el-date-picker v-model="form.endDate" type="datetime" style="width: 100%;" placeholder="Continuous if blank" />
            </el-form-item>
          </el-col>
        </el-row>

        <div class="form-actions">
          <el-button type="primary" size="large" :loading="saving" @click="handleUpdate">
            Save Changes
          </el-button>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import { campaignsApi } from '../api/campaigns';

const route = useRoute();
const router = useRouter();
const campaignId = route.params.id as string;

const formRef = ref<FormInstance>();
const loading = ref(false);
const saving = ref(false);

const form = reactive({
  name: '',
  description: '',
  status: 2,
  dailyBudget: 250,
  totalBudget: 5000,
  startDate: new Date(),
  endDate: null as Date | null
});

const rules: FormRules = {
  name: [{ required: true, message: 'Campaign name is required', trigger: 'blur' }],
  dailyBudget: [{ required: true, message: 'Daily budget is required', trigger: 'blur' }]
};

async function loadCampaign() {
  loading.value = true;
  try {
    const data = await campaignsApi.getCampaign(campaignId);
    form.name = data.name;
    form.description = data.description || '';
    form.status = data.status;
    form.dailyBudget = Number(data.dailyBudget);
    form.totalBudget = data.totalBudget ? Number(data.totalBudget) : 0;
    form.startDate = new Date(data.startDate);
    form.endDate = data.endDate ? new Date(data.endDate) : null;
  } catch (err: any) {
    ElMessage.error('Failed to load campaign details');
    router.push('/campaigns');
  } finally {
    loading.value = false;
  }
}

async function handleUpdate() {
  if (!formRef.value) return;
  await formRef.value.validate(async (valid) => {
    if (!valid) return;
    saving.value = true;
    try {
      await campaignsApi.updateCampaign(campaignId, {
        name: form.name,
        description: form.description || undefined,
        status: form.status,
        dailyBudget: form.dailyBudget,
        totalBudget: form.totalBudget || undefined,
        startDate: form.startDate.toISOString(),
        endDate: form.endDate ? form.endDate.toISOString() : undefined
      });

      ElMessage.success('Campaign updated successfully!');
      router.push(`/campaigns/${campaignId}`);
    } catch (err: any) {
      ElMessage.error(err.response?.data?.message || err.message || 'Failed to update campaign');
    } finally {
      saving.value = false;
    }
  });
}

onMounted(() => {
  loadCampaign();
});
</script>

<style scoped>
.campaign-form-view {
  padding: 24px;
  max-width: 900px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0 0 4px;
  font-size: 24px;
  font-weight: 800;
  color: #f8fafc;
  letter-spacing: -0.02em;
}

.subtitle {
  margin: 0;
  font-size: 13px;
  color: #94a3b8;
}

.form-card {
  border-radius: 16px;
  padding: 16px;
}

.form-actions {
  margin-top: 24px;
  text-align: right;
}
</style>
