import { createClient } from 'redis';
import logger from './logger.js';

let redisClient = null;

export async function initRedis() {
  const redisHost = process.env.REDIS_HOST || 'localhost';
  const redisPort = process.env.REDIS_PORT || 6379;
  const redisPassword = process.env.REDIS_PASSWORD || '';

  redisClient = createClient({
    socket: {
      host: redisHost,
      port: redisPort
    },
    password: redisPassword || undefined
  });

  redisClient.on('error', (err) => {
    logger.error('Redis Client Error', { error: err.message });
  });

  redisClient.on('connect', () => {
    logger.info('Redis Client Connected', { host: redisHost, port: redisPort });
  });

  redisClient.on('ready', () => {
    logger.info('Redis Client Ready');
  });

  redisClient.on('reconnecting', () => {
    logger.warn('Redis Client Reconnecting');
  });

  await redisClient.connect();
  
  return redisClient;
}

export function getRedisClient() {
  if (!redisClient) {
    throw new Error('Redis client not initialized. Call initRedis() first.');
  }
  return redisClient;
}

export async function closeRedis() {
  if (redisClient) {
    await redisClient.quit();
    logger.info('Redis connection closed');
  }
}
