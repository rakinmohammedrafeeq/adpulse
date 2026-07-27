import { v4 as uuidv4 } from 'uuid';
import { getRedisClient } from '../config/redis.js';
import logger from '../config/logger.js';
import axios from 'axios';

const REDIS_QUEUE_KEY = 'adpulse:events:queue';
const REDIS_STATS_KEY = 'adpulse:stats';
const REDIS_EVENT_CACHE_PREFIX = 'adpulse:event:';
const EVENT_CACHE_TTL = 3600; // 1 hour

/**
 * Process and enqueue ad event
 */
export async function processEvent(eventData) {
  try {
    const redis = getRedisClient();
    
    // Enrich event with ID and timestamp if not present
    const event = {
      id: eventData.id || uuidv4(),
      ...eventData,
      eventTime: eventData.eventTime || new Date().toISOString(),
      processedAt: new Date().toISOString()
    };

    // Push event to Redis queue
    await redis.rPush(REDIS_QUEUE_KEY, JSON.stringify(event));
    
    // Cache event for quick access
    const cacheKey = `${REDIS_EVENT_CACHE_PREFIX}${event.id}`;
    await redis.setEx(cacheKey, EVENT_CACHE_TTL, JSON.stringify(event));
    
    // Increment stats
    await incrementStats(event.eventType, event.tenantId);
    
    logger.info('Event processed and queued', {
      eventId: event.id,
      eventType: event.eventType,
      tenantId: event.tenantId,
      campaignId: event.campaignId
    });
    
    return event;
  } catch (error) {
    logger.error('Error processing event', { error: error.message, eventData });
    throw error;
  }
}

/**
 * Process batch of events
 */
export async function processBatchEvents(events) {
  const results = {
    successful: 0,
    failed: 0,
    errors: []
  };

  for (const eventData of events) {
    try {
      await processEvent(eventData);
      results.successful++;
    } catch (error) {
      results.failed++;
      results.errors.push({
        event: eventData,
        error: error.message
      });
    }
  }

  logger.info('Batch events processed', {
    total: events.length,
    successful: results.successful,
    failed: results.failed
  });

  return results;
}

/**
 * Consume events from queue and send to API
 */
export async function consumeEvents() {
  const redis = getRedisClient();
  const apiBaseUrl = process.env.API_BASE_URL || 'http://localhost:5000';
  const batchSize = parseInt(process.env.BATCH_SIZE || '100');

  try {
    // Get batch of events from queue
    const events = [];
    for (let i = 0; i < batchSize; i++) {
      const eventJson = await redis.lPop(REDIS_QUEUE_KEY);
      if (!eventJson) break;
      
      try {
        events.push(JSON.parse(eventJson));
      } catch (parseError) {
        logger.error('Failed to parse event from queue', { error: parseError.message });
      }
    }

    if (events.length === 0) {
      return { processed: 0 };
    }

    // Forward events to API
    const eventTypeMap = {
      'impression': 0,
      'click': 1,
      'conversion': 2,
      'video_view': 3,
      'video_complete': 4,
      'app_install': 5
    };

    try {
      const formattedEvents = events.map(e => ({
        campaignId: e.campaignId,
        adGroupId: e.adGroupId || null,
        creativeId: e.creativeId || null,
        eventType: typeof e.eventType === 'number' ? e.eventType : (eventTypeMap[e.eventType] ?? 0),
        eventTime: e.eventTime || new Date().toISOString(),
        userId: e.userId,
        sessionId: e.sessionId,
        ipAddress: e.ipAddress,
        userAgent: e.userAgent,
        deviceType: e.deviceType || 'Desktop',
        country: e.country || 'United States',
        city: e.city,
        referrer: e.referrer,
        conversionValue: e.conversionValue,
        customData: typeof e.customData === 'object' ? JSON.stringify(e.customData) : e.customData
      }));

      const apiResponse = await axios.post(`${apiBaseUrl}/api/events/batch`, { events: formattedEvents }, {
        headers: { 'Content-Type': 'application/json' },
        timeout: 5000
      });
      logger.info('Forwarded consumed events to API', { count: events.length, apiStatus: apiResponse.status });
    } catch (apiError) {
      logger.warn('Could not forward batch to API; retaining in Redis analytics cache.', { error: apiError.message });
    }

    for (const event of events) {
      // Store in Redis for analytics/retrieval
      await storeEventForAnalytics(event);
    }

    logger.info('Events consumed from queue', { count: events.length });

    return { processed: events.length };
  } catch (error) {
    logger.error('Error consuming events', { error: error.message });
    throw error;
  }
}

/**
 * Store event data for analytics
 */
async function storeEventForAnalytics(event) {
  const redis = getRedisClient();
  
  // Store in time-series sorted sets for analytics
  const dateKey = new Date(event.eventTime).toISOString().split('T')[0]; // YYYY-MM-DD
  const analyticsKey = `adpulse:analytics:${event.tenantId}:${event.campaignId}:${dateKey}`;
  
  await redis.zAdd(analyticsKey, {
    score: new Date(event.eventTime).getTime(),
    value: JSON.stringify({
      id: event.id,
      type: event.eventType,
      adGroupId: event.adGroupId,
      creativeId: event.creativeId,
      conversionValue: event.conversionValue
    })
  });
  
  // Set expiry for 90 days
  await redis.expire(analyticsKey, 90 * 24 * 60 * 60);
}

/**
 * Increment statistics counters
 */
async function incrementStats(eventType, tenantId) {
  const redis = getRedisClient();
  const today = new Date().toISOString().split('T')[0];
  
  // Global stats
  await redis.hIncrBy(`${REDIS_STATS_KEY}:global:${today}`, eventType, 1);
  await redis.hIncrBy(`${REDIS_STATS_KEY}:global:${today}`, 'total', 1);
  
  // Tenant-specific stats
  await redis.hIncrBy(`${REDIS_STATS_KEY}:tenant:${tenantId}:${today}`, eventType, 1);
  await redis.hIncrBy(`${REDIS_STATS_KEY}:tenant:${tenantId}:${today}`, 'total', 1);
}

/**
 * Get current queue size
 */
export async function getQueueSize() {
  const redis = getRedisClient();
  return await redis.lLen(REDIS_QUEUE_KEY);
}

/**
 * Get statistics
 */
export async function getStats(tenantId = null) {
  const redis = getRedisClient();
  const today = new Date().toISOString().split('T')[0];
  
  const statsKey = tenantId 
    ? `${REDIS_STATS_KEY}:tenant:${tenantId}:${today}`
    : `${REDIS_STATS_KEY}:global:${today}`;
  
  const stats = await redis.hGetAll(statsKey);
  
  return {
    date: today,
    queueSize: await getQueueSize(),
    ...stats
  };
}

/**
 * Start background queue consumer
 */
export function startQueueConsumer(intervalMs = 5000) {
  logger.info('Starting queue consumer', { intervalMs });
  
  const consume = async () => {
    try {
      const result = await consumeEvents();
      if (result.processed > 0) {
        logger.debug('Queue consumer iteration completed', result);
      }
    } catch (error) {
      logger.error('Queue consumer error', { error: error.message });
    }
  };

  // Initial consumption
  consume();
  
  // Set up interval
  const intervalId = setInterval(consume, intervalMs);
  
  return intervalId;
}
