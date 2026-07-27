import express from 'express';
import { v4 as uuidv4 } from 'uuid';
import { processEvent, consumeEvents } from '../services/eventProcessor.js';
import logger from '../config/logger.js';

const router = express.Router();

const DEMO_TENANT_ID = 'e7b1a2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c';
const DEMO_CAMPAIGN_ID = 'c1d2e3f4-a5b6-4c7d-8e9f-0a1b2c3d4e5f';

const DEVICES = ['Desktop', 'Mobile', 'Tablet'];
const COUNTRIES = ['United States', 'United Kingdom', 'Germany', 'Canada'];
const CITIES = ['New York', 'London', 'Berlin', 'Toronto', 'San Francisco'];

/**
 * Generate simulated ad traffic (impressions, clicks, conversions)
 * POST /events/simulate
 */
router.post('/simulate', async (req, res) => {
  try {
    const count = Math.min(parseInt(req.body.count || '25'), 200);
    const tenantId = req.body.tenantId || DEMO_TENANT_ID;
    const campaignId = req.body.campaignId || DEMO_CAMPAIGN_ID;

    const generated = [];
    let impressions = 0;
    let clicks = 0;
    let conversions = 0;

    for (let i = 0; i < count; i++) {
      const now = new Date();
      const userId = `sim_user_${Math.floor(Math.random() * 9000 + 1000)}`;
      const sessionId = uuidv4().substring(0, 12);
      const device = DEVICES[Math.floor(Math.random() * DEVICES.length)];
      const country = COUNTRIES[Math.floor(Math.random() * COUNTRIES.length)];
      const city = CITIES[Math.floor(Math.random() * CITIES.length)];

      // 1. Impression
      const impression = {
        id: uuidv4(),
        tenantId,
        campaignId,
        eventType: 'impression',
        eventTime: now.toISOString(),
        userId,
        sessionId,
        deviceType: device,
        country,
        city,
        ipAddress: `10.0.${Math.floor(Math.random() * 255)}.${Math.floor(Math.random() * 255)}`
      };
      await processEvent(impression);
      impressions++;
      generated.push(impression);

      // 2. Click (approx 15% probability)
      if (Math.random() < 0.15) {
        const clickTime = new Date(now.getTime() + Math.floor(Math.random() * 15000 + 2000));
        const click = {
          id: uuidv4(),
          tenantId,
          campaignId,
          eventType: 'click',
          eventTime: clickTime.toISOString(),
          userId,
          sessionId,
          deviceType: device,
          country,
          city,
          ipAddress: impression.ipAddress
        };
        await processEvent(click);
        clicks++;
        generated.push(click);

        // 3. Conversion (approx 25% of clicks)
        if (Math.random() < 0.25) {
          const convTime = new Date(clickTime.getTime() + Math.floor(Math.random() * 60000 + 5000));
          const conversion = {
            id: uuidv4(),
            tenantId,
            campaignId,
            eventType: 'conversion',
            eventTime: convTime.toISOString(),
            userId,
            sessionId,
            deviceType: device,
            country,
            city,
            conversionValue: Math.round((Math.random() * 200 + 49.99) * 100) / 100
          };
          await processEvent(conversion);
          conversions++;
          generated.push(conversion);
        }
      }
    }

    // Trigger immediate batch consumption from Redis
    const consumed = await consumeEvents();

    logger.info('Simulated events generated', {
      total: generated.length,
      impressions,
      clicks,
      conversions,
      consumed: consumed.processed
    });

    res.json({
      success: true,
      message: `Generated and queued ${generated.length} ad events through Redis.`,
      stats: {
        totalEvents: generated.length,
        impressions,
        clicks,
        conversions,
        processedFromQueue: consumed.processed
      }
    });
  } catch (error) {
    logger.error('Error simulating events', { error: error.message });
    res.status(500).json({ error: error.message });
  }
});

export default router;
