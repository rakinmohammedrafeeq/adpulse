import client from './client';
import type { AudienceDto } from '../types';

export const audiencesApi = {
  getAudiences: async (): Promise<AudienceDto[]> => {
    const res = await client.get<AudienceDto[]>('/api/audiences');
    return res.data;
  },

  getAudience: async (id: string): Promise<AudienceDto> => {
    const res = await client.get<AudienceDto>(`/api/audiences/${id}`);
    return res.data;
  },

  createAudience: async (payload: {
    name: string;
    description?: string;
    type: number;
    demographics?: string;
    interests?: string;
    behaviors?: string;
    locations?: string;
    devices?: string;
    estimatedSize: number;
  }): Promise<AudienceDto> => {
    const res = await client.post<AudienceDto>('/api/audiences', payload);
    return res.data;
  },

  updateAudience: async (
    id: string,
    payload: {
      name?: string;
      description?: string;
      type?: number;
      demographics?: string;
      interests?: string;
      behaviors?: string;
      locations?: string;
      devices?: string;
      estimatedSize?: number;
    }
  ): Promise<AudienceDto> => {
    const res = await client.put<AudienceDto>(`/api/audiences/${id}`, payload);
    return res.data;
  },

  deleteAudience: async (id: string): Promise<void> => {
    await client.delete(`/api/audiences/${id}`);
  }
};
