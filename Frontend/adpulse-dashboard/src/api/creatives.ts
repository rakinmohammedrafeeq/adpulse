import client from './client';
import type { CreativeListDto, CreativeDetailDto } from '../types';

export const creativesApi = {
  getCreatives: async (adGroupId: string): Promise<CreativeListDto[]> => {
    const res = await client.get<CreativeListDto[]>(`/api/adgroups/${adGroupId}/creatives`);
    return res.data;
  },

  getCreative: async (id: string): Promise<CreativeDetailDto> => {
    const res = await client.get<CreativeDetailDto>(`/api/creatives/${id}`);
    return res.data;
  },

  createCreative: async (
    adGroupId: string,
    payload: {
      name: string;
      type: number;
      headline: string;
      description?: string;
      imageUrl?: string;
      videoUrl?: string;
      destinationUrl: string;
      callToAction?: string;
      width?: number;
      height?: number;
    }
  ): Promise<CreativeDetailDto> => {
    const res = await client.post<CreativeDetailDto>(`/api/adgroups/${adGroupId}/creatives`, payload);
    return res.data;
  },

  updateCreative: async (
    id: string,
    payload: {
      name?: string;
      status?: number;
      headline?: string;
      description?: string;
      imageUrl?: string;
      videoUrl?: string;
      destinationUrl?: string;
      callToAction?: string;
    }
  ): Promise<CreativeDetailDto> => {
    const res = await client.put<CreativeDetailDto>(`/api/creatives/${id}`, payload);
    return res.data;
  },

  deleteCreative: async (id: string): Promise<void> => {
    await client.delete(`/api/creatives/${id}`);
  }
};
