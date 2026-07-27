import Joi from 'joi';

// Event types
export const EVENT_TYPES = {
  IMPRESSION: 'impression',
  CLICK: 'click',
  CONVERSION: 'conversion',
  VIDEO_VIEW: 'video_view',
  VIDEO_COMPLETE: 'video_complete',
  APP_INSTALL: 'app_install'
};

// Base event schema
const baseEventSchema = Joi.object({
  tenantId: Joi.string().uuid().required(),
  campaignId: Joi.string().uuid().required(),
  adGroupId: Joi.string().uuid().optional(),
  creativeId: Joi.string().uuid().optional(),
  eventType: Joi.string().valid(...Object.values(EVENT_TYPES)).required(),
  eventTime: Joi.date().iso().optional(),
  
  // User context
  userId: Joi.string().max(100).optional(),
  sessionId: Joi.string().max(100).optional(),
  ipAddress: Joi.string().ip().optional(),
  userAgent: Joi.string().max(500).optional(),
  deviceType: Joi.string().max(50).optional(),
  country: Joi.string().max(100).optional(),
  city: Joi.string().max(100).optional(),
  
  // Event metadata
  referrer: Joi.string().uri().max(2000).optional(),
  conversionValue: Joi.number().positive().optional(),
  customData: Joi.object().optional()
});

// Batch event schema
export const batchEventSchema = Joi.object({
  events: Joi.array().items(baseEventSchema).min(1).max(1000).required()
});

// Single event schema
export const singleEventSchema = baseEventSchema;

// Validation function
export function validateEvent(event, isBatch = false) {
  const schema = isBatch ? batchEventSchema : singleEventSchema;
  const { error, value } = schema.validate(event, { 
    abortEarly: false,
    stripUnknown: true 
  });
  
  if (error) {
    const errors = error.details.map(detail => ({
      field: detail.path.join('.'),
      message: detail.message
    }));
    return { valid: false, errors };
  }
  
  return { valid: true, value };
}
