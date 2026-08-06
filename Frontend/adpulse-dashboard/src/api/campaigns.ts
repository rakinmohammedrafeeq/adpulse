import client from './client';
import type { CampaignListDto, CampaignDetailDto } from '../types';

export const campaignsApi = {
  getCampaigns: async (): Promise<CampaignListDto[]> => {
    const res = await client.get<CampaignListDto[]>('/api/campaigns');
    return res.data;
  },

  getCampaign: async (id: string): Promise<CampaignDetailDto> => {
    const res = await client.get<CampaignDetailDto>(`/api/campaigns/${id}`);
    return res.data;
  },

  createCampaign: async (payload: {
    name: string;
    description?: string;
    objective: number;
    dailyBudget: number;
    totalBudget?: number;
    startDate: string;
    endDate?: string;
  }): Promise<CampaignDetailDto> => {
    const res = await client.post<CampaignDetailDto>('/api/campaigns', payload);
    return res.data;
  },

  updateCampaign: async (
    id: string,
    payload: {
      name?: string;
      description?: string;
      status?: number;
      dailyBudget?: number;
      totalBudget?: number;
      startDate?: string;
      endDate?: string;
    }
  ): Promise<CampaignDetailDto> => {
    const res = await client.put<CampaignDetailDto>(`/api/campaigns/${id}`, payload);
    return res.data;
  },

  deleteCampaign: async (id: string): Promise<void> => {
    await client.delete(`/api/campaigns/${id}`);
  }
};
