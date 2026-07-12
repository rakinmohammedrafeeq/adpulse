const express = require('express');
const router = express.Router();
const logger = require('../utils/logger');

let metrics = {
  impressions: 0,
  clicks: 0,
  conversions: 0,
  startTime: Date.now()
};

// GET /api/metrics - Get current metrics
router.get('/', (req, res) => {
  const uptime = Math.floor((Date.now() - metrics.startTime) / 1000);
  
  res.json({
    ...metrics,
    uptime,
    timestamp: new Date().toISOString()
  });
});

// POST /api/metrics/increment - Internal use for incrementing metrics
router.post('/increment/:type', (req, res) => {
  const { type } = req.params;
  
  if (metrics.hasOwnProperty(type)) {
    metrics[type]++;
    res.json({ success: true, [type]: metrics[type] });
  } else {
    res.status(400).json({ success: false, error: 'Invalid metric type' });
  }
});

module.exports = router;
