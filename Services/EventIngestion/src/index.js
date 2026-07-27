import 'dotenv/config';
import express from 'express';
import helmet from 'helmet';
import compression from 'compression';
import cors from 'cors';
import morgan from 'morgan';
import { initRedis, closeRedis } from './config/redis.js';
import logger from './config/logger.js';
import { errorHandler, notFoundHandler } from './middleware/errorHandler.js';
import { startQueueConsumer } from './services/eventProcessor.js';
import eventsRouter from './routes/events.js';
import healthRouter from './routes/health.js';
import simulationRouter from './routes/simulation.js';

const app = express();
const PORT = process.env.EVENT_SERVICE_PORT || process.env.PORT || 3001;

// Middleware
app.use(helmet()); // Security headers
app.use(compression()); // Response compression
app.use(cors()); // CORS support
app.use(express.json({ limit: '10mb' })); // JSON body parser
app.use(express.urlencoded({ extended: true, limit: '10mb' }));

// Request logging
app.use(morgan('combined', {
  stream: {
    write: (message) => logger.info(message.trim())
  }
}));

// Routes
app.use('/events', eventsRouter);
app.use('/events', simulationRouter);
app.use('/health', healthRouter);

// Root endpoint
app.get('/', (req, res) => {
  res.json({
    service: 'AdPulse Event Ingestion Service',
    version: '1.0.0',
    status: 'running',
    endpoints: {
      health: '/health',
      stats: '/health/stats',
      queue: '/health/queue',
      ingestEvent: 'POST /events',
      ingestBatch: 'POST /events/batch'
    }
  });
});

// Error handlers
app.use(notFoundHandler);
app.use(errorHandler);

// Graceful shutdown
let queueConsumerInterval;

async function shutdown() {
  logger.info('Shutting down gracefully...');
  
  // Stop queue consumer
  if (queueConsumerInterval) {
    clearInterval(queueConsumerInterval);
    logger.info('Queue consumer stopped');
  }
  
  // Close Redis connection
  await closeRedis();
  
  process.exit(0);
}

process.on('SIGTERM', shutdown);
process.on('SIGINT', shutdown);

// Start server
async function start() {
  try {
    // Initialize Redis
    await initRedis();
    logger.info('Redis initialized successfully');

    // Start queue consumer
    const consumerInterval = parseInt(process.env.CONSUMER_INTERVAL_MS || '5000');
    queueConsumerInterval = startQueueConsumer(consumerInterval);
    logger.info('Queue consumer started', { intervalMs: consumerInterval });

    // Start Express server
    app.listen(PORT, () => {
      logger.info(`Event Ingestion Service started`, {
        port: PORT,
        environment: process.env.NODE_ENV || 'development',
        nodeVersion: process.version
      });
      logger.info(`Service available at http://localhost:${PORT}`);
    });
  } catch (error) {
    logger.error('Failed to start service', { error: error.message });
    process.exit(1);
  }
}

start();
