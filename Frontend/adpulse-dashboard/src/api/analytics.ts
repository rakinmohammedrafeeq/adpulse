import client from './client';
import type { DashboardStatsDto, CampaignDetailDto, TimeSeriesDataPoint } from '../types';

export const analyticsApi = {
  getDashboardStats: async (): Promise<DashboardStatsDto> => {
    const res = await client.get<DashboardStatsDto>('/api/analytics/dashboard');
    return res.data;
  },

  getCampaignAnalytics: async (
    campaignId: string,
    startDate?: string,
    endDate?: string
  ): Promise<any> => {
    const res = await client.get(`/api/analytics/campaigns/${campaignId}`, {
      params: { startDate, endDate }
    });
    return res.data;
  },

  getCampaignTimeSeries: async (
    campaignId: string,
    startDate?: string,
    endDate?: string
  ): Promise<TimeSeriesDataPoint[]> => {
    const res = await client.get<TimeSeriesDataPoint[]>(`/api/analytics/campaigns/${campaignId}/timeseries`, {
      params: { startDate, endDate }
    });
    return res.data;
  },

  searchCampaigns: async (query: string): Promise<{ total: number; results: CampaignDetailDto[] }> => {
    const res = await client.get('/api/search/campaigns', {
      params: { q: query }
    });
    return res.data;
  }
};
