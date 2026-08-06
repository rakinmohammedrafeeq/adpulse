export interface User {
  userId: string;
  email: string;
  role: string;
  tenantId: string;
  firstName?: string;
  lastName?: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  tenantId: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
}

export type CampaignStatus = 0 | 1 | 2 | 3 | 4 | 5; // Draft, Scheduled, Active, Paused, Completed, Archived
export type CampaignObjective = 0 | 1 | 2 | 3 | 4 | 5; // BrandAwareness, Reach, Traffic, Engagement, Conversions, AppPromotion

export const CampaignStatusLabels: Record<number, string> = {
  0: 'Draft',
  1: 'Scheduled',
  2: 'Active',
  3: 'Paused',
  4: 'Completed',
  5: 'Archived'
};

export const CampaignStatusTagType: Record<number, 'info' | 'warning' | 'success' | 'danger' | 'primary'> = {
  0: 'info',
  1: 'warning',
  2: 'success',
  3: 'warning',
  4: 'info',
  5: 'danger'
};

export const CampaignObjectiveLabels: Record<number, string> = {
  0: 'Brand Awareness',
  1: 'Reach',
  2: 'Website Traffic',
  3: 'Engagement',
  4: 'Conversions',
  5: 'App Promotion'
};

export interface CampaignListDto {
  id: string;
  name: string;
  status: number;
  objective: number;
  dailyBudget: number;
  spentAmount: number;
  startDate: string;
  endDate?: string;
  createdAt: string;
}

export interface CampaignDetailDto {
  id: string;
  name: string;
  description?: string;
  status: number;
  objective: number;
  dailyBudget: number;
  totalBudget?: number;
  spentAmount: number;
  startDate: string;
  endDate?: string;
  createdAt: string;
  updatedAt?: string;
  adGroups: AdGroupListDto[];
}

export interface AdGroupListDto {
  id: string;
  name: string;
  status: number;
  biddingStrategy: number;
  bidAmount: number;
  createdAt: string;
}

export interface AdGroupDetailDto {
  id: string;
  name: string;
  status: number;
  biddingStrategy: number;
  bidAmount: number;
  targetingRules?: string;
  createdAt: string;
  updatedAt?: string;
  creatives: CreativeListDto[];
}

export interface CreativeListDto {
  id: string;
  name: string;
  type: number;
  status: number;
  headline: string;
  imageUrl?: string;
  createdAt: string;
}

export interface CreativeDetailDto {
  id: string;
  name: string;
  type: number;
  status: number;
  headline: string;
  description?: string;
  imageUrl?: string;
  videoUrl?: string;
  destinationUrl: string;
  callToAction?: string;
  width?: number;
  height?: number;
  createdAt: string;
  updatedAt?: string;
}

export interface AudienceDto {
  id: string;
  tenantId: string;
  name: string;
  description?: string;
  type: number;
  demographics?: string;
  interests?: string;
  behaviors?: string;
  locations?: string;
  devices?: string;
  estimatedSize: number;
  createdAt: string;
  updatedAt?: string;
}

export interface DashboardStatsDto {
  totalCampaigns: number;
  activeCampaigns: number;
  totalBudget: number;
  totalSpent: number;
  totalImpressions: number;
  totalClicks: number;
  totalConversions: number;
  averageCTR: number;
  averageCPC: number;
  totalRevenue: number;
  overallROAS: number;
}

export interface TimeSeriesDataPoint {
  date: string;
  impressions: number;
  clicks: number;
  conversions: number;
  spend: number;
  revenue: number;
}

export interface EventDto {
  id: string;
  tenantId: string;
  campaignId: string;
  adGroupId?: string;
  creativeId?: string;
  eventType: number;
  eventTime: string;
  userId?: string;
  deviceType?: string;
  country?: string;
  city?: string;
  conversionValue?: number;
}
