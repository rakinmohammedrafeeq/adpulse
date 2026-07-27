import express from 'express';
import { validateEvent } from '../validators/eventValidator.js';
import { processEvent, processBatchEvents } from '../services/eventProcessor.js';
import logger from '../config/logger.js';

const router = express.Router();

/**
 * POST /events - Ingest a single event
 */
router.post('/', async (req, res, next) => {
  try {
    // Validate event
    const validation = validateEvent(req.body, false);
    
    if (!validation.valid) {
      return res.status(400).json({
        error: 'Validation Error',
        errors: validation.errors
      });
    }

    // Process event
    const event = await processEvent(validation.value);

    res.status(202).json({
      message: 'Event accepted for processing',
      eventId: event.id
    });
  } catch (error) {
    next(error);
  }
});

/**
 * POST /events/batch - Ingest multiple events
 */
router.post('/batch', async (req, res, next) => {
  try {
    // Validate batch
    const validation = validateEvent(req.body, true);
    
    if (!validation.valid) {
      return res.status(400).json({
        error: 'Validation Error',
        errors: validation.errors
      });
    }

    // Process batch
    const results = await processBatchEvents(validation.value.events);

    res.status(202).json({
      message: 'Batch events accepted for processing',
      total: validation.value.events.length,
      successful: results.successful,
      failed: results.failed,
      errors: results.errors
    });
  } catch (error) {
    next(error);
  }
});

export default router;
