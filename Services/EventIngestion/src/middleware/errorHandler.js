import logger from '../config/logger.js';

export function errorHandler(err, req, res, next) {
  logger.error('Error handler caught exception', {
    error: err.message,
    stack: err.stack,
    path: req.path,
    method: req.method
  });

  // Validation errors
  if (err.name === 'ValidationError') {
    return res.status(400).json({
      error: 'Validation Error',
      message: err.message,
      details: err.details
    });
  }

  // Redis errors
  if (err.message && err.message.includes('Redis')) {
    return res.status(503).json({
      error: 'Service Unavailable',
      message: 'Redis connection error'
    });
  }

  // Default server error
  res.status(500).json({
    error: 'Internal Server Error',
    message: process.env.NODE_ENV === 'production' 
      ? 'An unexpected error occurred' 
      : err.message
  });
}

export function notFoundHandler(req, res) {
  res.status(404).json({
    error: 'Not Found',
    message: `Route ${req.method} ${req.path} not found`
  });
}
