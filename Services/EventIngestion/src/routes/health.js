import express from 'express';
import { getRedisClient } from '../config/redis.js';
import { getQueueSize, getStats } from '../services/eventProcessor.js';

const router = express.Router();

/**
 * GET /health - Health check endpoint
 */
router.get('/', async (req, res) => {
  try {
    const redis = getRedisClient();
    
    // Check Redis connection
    const redisPing = await redis.ping();
    const redisConnected = redisPing === 'PONG';

    const health = {
      status: redisConnected ? 'healthy' : 'degraded',
      timestamp: new Date().toISOString(),
      service: 'event-ingestion',
      version: '1.0.0',
      checks: {
        redis: redisConnected ? 'up' : 'down'
      }
    };

    const statusCode = redisConnected ? 200 : 503;
    res.status(statusCode).json(health);
  } catch (error) {
    res.status(503).json({
      status: 'unhealthy',
      timestamp: new Date().toISOString(),
      service: 'event-ingestion',
      error: error.message
    });
  }
});

/**
 * GET /health/stats - Get service statistics
 */
router.get('/stats', async (req, res, next) => {
  try {
    const { tenantId } = req.query;
    const stats = await getStats(tenantId || null);

    res.json(stats);
  } catch (error) {
    next(error);
  }
});

/**
 * GET /health/queue - Get queue information
 */
router.get('/queue', async (req, res, next) => {
  try {
    const queueSize = await getQueueSize();

    res.json({
      queueSize,
      timestamp: new Date().toISOString()
    });
  } catch (error) {
    next(error);
  }
});

export default router;
