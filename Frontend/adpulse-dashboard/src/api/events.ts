import client from './client';
import axios from 'axios';
import type { EventDto } from '../types';

const eventServiceUrl = import.meta.env.VITE_EVENT_SERVICE_URL || 'http://localhost:3001';

export const eventsApi = {
  getRecentEvents: async (campaignId?: string, limit: number = 50): Promise<EventDto[]> => {
    const res = await client.get<EventDto[]>('/api/events', {
      params: { campaignId, limit }
    });
    return res.data;
  },

  simulateEvents: async (
    count: number = 25,
    tenantId?: string,
    campaignId?: string
  ): Promise<{ success: boolean; message: string; stats?: any }> => {
    try {
      // First try calling Node.js event ingestion service directly
      const response = await axios.post(`${eventServiceUrl}/events/simulate`, {
        count,
        tenantId,
        campaignId
      }, { timeout: 4000 });
      return response.data;
    } catch (e) {
      // Fallback: Ingest batch directly via ASP.NET Core API
      const events: any[] = [];
      const cId = campaignId || 'c1d2e3f4-a5b6-4c7d-8e9f-0a1b2c3d4e5f';
      const devices = ['Desktop', 'Mobile', 'Tablet'];
      const countries = ['United States', 'United Kingdom', 'Germany', 'Canada'];
      const cities = ['New York', 'London', 'Berlin', 'Toronto'];

      for (let i = 0; i < count; i++) {
        const uId = `sim_user_${Math.floor(Math.random() * 9000 + 1000)}`;
        const dev = devices[Math.floor(Math.random() * devices.length)];
        const cntry = countries[Math.floor(Math.random() * countries.length)];
        const cty = cities[Math.floor(Math.random() * cities.length)];

        // Impression
        events.push({
          campaignId: cId,
          eventType: 0,
          eventTime: new Date().toISOString(),
          userId: uId,
          deviceType: dev,
          country: cntry,
          city: cty
        });

        // Click
        if (Math.random() < 0.2) {
          events.push({
            campaignId: cId,
            eventType: 1,
            eventTime: new Date().toISOString(),
            userId: uId,
            deviceType: dev,
            country: cntry,
            city: cty
          });

          // Conversion
          if (Math.random() < 0.25) {
            events.push({
              campaignId: cId,
              eventType: 2,
              eventTime: new Date().toISOString(),
              userId: uId,
              deviceType: dev,
              country: cntry,
              city: cty,
              conversionValue: Math.round((Math.random() * 150 + 50) * 100) / 100
            });
          }
        }
      }

      const res = await client.post('/api/events/batch', { events });
      return {
        success: true,
        message: `Generated and ingested ${events.length} events directly via API.`,
        stats: res.data
      };
    }
  }
};
