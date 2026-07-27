import axios from 'axios';

const SERVICE_URL = process.env.EVENT_SERVICE_URL || 'http://localhost:3001';
const COUNT = process.argv[2] ? parseInt(process.argv[2]) : 30;

console.log(`[AdPulse Event Simulator] Triggering generation of ${COUNT} events to ${SERVICE_URL}/events/simulate ...`);

try {
  const response = await axios.post(`${SERVICE_URL}/events/simulate`, { count: COUNT });
  console.log('[AdPulse Event Simulator] Success!');
  console.log(JSON.stringify(response.data, null, 2));
} catch (error) {
  console.error('[AdPulse Event Simulator] Failed to contact Event Ingestion Service:');
  if (error.response) {
    console.error(`Status ${error.response.status}:`, error.response.data);
  } else {
    console.error(error.message);
  }
  process.exit(1);
}
