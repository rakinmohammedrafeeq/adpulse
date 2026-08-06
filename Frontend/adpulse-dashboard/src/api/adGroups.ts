import client from './client';
import type { AdGroupListDto, AdGroupDetailDto } from '../types';

export const adGroupsApi = {
  getAdGroups: async (campaignId: string): Promise<AdGroupListDto[]> => {
    const res = await client.get<AdGroupListDto[]>(`/api/campaigns/${campaignId}/adgroups`);
    return res.data;
  },

  getAdGroup: async (id: string): Promise<AdGroupDetailDto> => {
    const res = await client.get<AdGroupDetailDto>(`/api/adgroups/${id}`);
    return res.data;
  },

  createAdGroup: async (
    campaignId: string,
    payload: {
      name: string;
      biddingStrategy: number;
      bidAmount: number;
      targetingRules?: string;
    }
  ): Promise<AdGroupDetailDto> => {
    const res = await client.post<AdGroupDetailDto>(`/api/campaigns/${campaignId}/adgroups`, payload);
    return res.data;
  },

  updateAdGroup: async (
    id: string,
    payload: {
      name?: string;
      status?: number;
      biddingStrategy?: number;
      bidAmount?: number;
      targetingRules?: string;
    }
  ): Promise<AdGroupDetailDto> => {
    const res = await client.put<AdGroupDetailDto>(`/api/adgroups/${id}`, payload);
    return res.data;
  },

  deleteAdGroup: async (id: string): Promise<void> => {
    await client.delete(`/api/adgroups/${id}`);
  }
};
