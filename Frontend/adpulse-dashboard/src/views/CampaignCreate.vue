<template>
  <div class="campaign-form-view">
    <div class="page-header">
      <div>
        <h2>Create Advertising Campaign</h2>
        <p class="subtitle">Set campaign objective, budget pacing, and scheduling configuration</p>
      </div>

      <el-button @click="$router.push('/campaigns')">Cancel</el-button>
    </div>

    <el-card shadow="hover" class="form-card">
      <el-form :model="form" :rules="rules" ref="formRef" label-position="top">
        <el-row :gutter="20">
          <el-col :span="16">
            <el-form-item label="Campaign Name" prop="name">
              <el-input v-model="form.name" placeholder="e.g. Q4 Growth Acquisition - US Search" />
            </el-form-item>
          </el-col>

          <el-col :span="8">
            <el-form-item label="Objective" prop="objective">
              <el-select v-model="form.objective" style="width: 100%;">
                <el-option :value="0" label="Brand Awareness" />
                <el-option :value="1" label="Reach" />
                <el-option :value="2" label="Website Traffic" />
                <el-option :value="3" label="Engagement" />
                <el-option :value="4" label="Conversions" />
                <el-option :value="5" label="App Promotion" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="Description" prop="description">
          <el-input
            v-model="form.description"
            type="textarea"
            :rows="3"
            placeholder="Describe the campaign target, key value propositions, and promotion details..."
          />
        </el-form-item>

        <el-divider content-position="left">Budget & Pacing</el-divider>

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
          <el-button type="primary" size="large" :loading="saving" @click="handleSubmit">
            Create Campaign
          </el-button>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import { campaignsApi } from '../api/campaigns';

const router = useRouter();
const formRef = ref<FormInstance>();
const saving = ref(false);

const form = reactive({
  name: '',
  description: '',
  objective: 4, // Conversions
  dailyBudget: 250,
  totalBudget: 5000,
  startDate: new Date(),
  endDate: null as Date | null
});

const rules: FormRules = {
  name: [{ required: true, message: 'Campaign name is required', trigger: 'blur' }],
  objective: [{ required: true, message: 'Objective is required', trigger: 'change' }],
  dailyBudget: [{ required: true, message: 'Daily budget is required', trigger: 'blur' }],
  startDate: [{ required: true, message: 'Start date is required', trigger: 'change' }]
};

async function handleSubmit() {
  if (!formRef.value) return;
  await formRef.value.validate(async (valid) => {
    if (!valid) return;
    saving.value = true;
    try {
      const created = await campaignsApi.createCampaign({
        name: form.name,
        description: form.description || undefined,
        objective: form.objective,
        dailyBudget: form.dailyBudget,
        totalBudget: form.totalBudget || undefined,
        startDate: form.startDate.toISOString(),
        endDate: form.endDate ? form.endDate.toISOString() : undefined
      });

      ElMessage.success('Campaign created successfully!');
      router.push(`/campaigns/${created.id}`);
    } catch (err: any) {
      ElMessage.error(err.response?.data?.message || err.message || 'Failed to create campaign');
    } finally {
      saving.value = false;
    }
  });
}
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
