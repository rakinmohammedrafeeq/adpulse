-- AdPulse Database Schema - Initial Migration
-- Run this script against your SQL Server database

-- Create Tenants table
CREATE TABLE [Tenants] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL UNIQUE,
    [CompanyName] NVARCHAR(200) NOT NULL,
    [Website] NVARCHAR(500) NULL,
    [Industry] NVARCHAR(100) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL
);

CREATE INDEX [IX_Tenants_Name] ON [Tenants] ([Name]);

-- Create Users table
CREATE TABLE [Users] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'User',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastLoginAt] DATETIME2 NULL,
    CONSTRAINT [FK_Users_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Users_Email_TenantId] ON [Users] ([Email], [TenantId]);
CREATE INDEX [IX_Users_TenantId] ON [Users] ([TenantId]);

-- Create Campaigns table
CREATE TABLE [Campaigns] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [Status] INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Scheduled, 2=Active, 3=Paused, 4=Completed, 5=Archived
    [Objective] INT NOT NULL, -- 0=BrandAwareness, 1=Reach, 2=Traffic, 3=Engagement, 4=Conversions, etc.
    [DailyBudget] DECIMAL(18,2) NOT NULL,
    [TotalBudget] DECIMAL(18,2) NULL,
    [SpentAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Campaigns_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Campaigns_TenantId_Name] ON [Campaigns] ([TenantId], [Name]);
CREATE INDEX [IX_Campaigns_TenantId_Status] ON [Campaigns] ([TenantId], [Status]);

-- Create AdGroups table
CREATE TABLE [AdGroups] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CampaignId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Status] INT NOT NULL DEFAULT 0, -- 0=Active, 1=Paused, 2=Archived
    [BiddingStrategy] INT NOT NULL, -- 0=ManualCPC, 1=ManualCPM, etc.
    [BidAmount] DECIMAL(18,2) NOT NULL,
    [TargetingRules] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_AdGroups_Campaigns] FOREIGN KEY ([CampaignId]) REFERENCES [Campaigns]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AdGroups_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_AdGroups_TenantId_CampaignId] ON [AdGroups] ([TenantId], [CampaignId]);

-- Create Creatives table
CREATE TABLE [Creatives] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [AdGroupId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Type] INT NOT NULL, -- 0=Image, 1=Video, 2=Carousel, 3=Native, 4=Text, 5=ResponsiveDisplay
    [Status] INT NOT NULL DEFAULT 0, -- 0=Active, 1=Paused, 2=Rejected, 3=Archived
    [Headline] NVARCHAR(500) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [ImageUrl] NVARCHAR(1000) NULL,
    [VideoUrl] NVARCHAR(1000) NULL,
    [DestinationUrl] NVARCHAR(2000) NOT NULL,
    [CallToAction] NVARCHAR(100) NULL,
    [Width] INT NULL,
    [Height] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Creatives_AdGroups] FOREIGN KEY ([AdGroupId]) REFERENCES [AdGroups]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Creatives_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Creatives_TenantId_AdGroupId] ON [Creatives] ([TenantId], [AdGroupId]);

-- Create Audiences table
CREATE TABLE [Audiences] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [Type] INT NOT NULL, -- 0=Custom, 1=Lookalike, 2=Retargeting, etc.
    [Demographics] NVARCHAR(MAX) NULL,
    [Interests] NVARCHAR(MAX) NULL,
    [Behaviors] NVARCHAR(MAX) NULL,
    [Locations] NVARCHAR(MAX) NULL,
    [Devices] NVARCHAR(MAX) NULL,
    [EstimatedSize] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Audiences_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Audiences_TenantId_Name] ON [Audiences] ([TenantId], [Name]);

-- Create AdGroupAudience junction table (many-to-many)
CREATE TABLE [AdGroupAudience] (
    [AdGroupId] UNIQUEIDENTIFIER NOT NULL,
    [AudienceId] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY ([AdGroupId], [AudienceId]),
    CONSTRAINT [FK_AdGroupAudience_AdGroups] FOREIGN KEY ([AdGroupId]) REFERENCES [AdGroups]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AdGroupAudience_Audiences] FOREIGN KEY ([AudienceId]) REFERENCES [Audiences]([Id]) ON DELETE CASCADE
);

-- Create AdEvents table
CREATE TABLE [AdEvents] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CampaignId] UNIQUEIDENTIFIER NOT NULL,
    [AdGroupId] UNIQUEIDENTIFIER NULL,
    [CreativeId] UNIQUEIDENTIFIER NULL,
    [EventType] INT NOT NULL, -- 0=Impression, 1=Click, 2=Conversion, 3=VideoView, 4=VideoComplete, 5=AppInstall
    [EventTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UserId] NVARCHAR(100) NULL,
    [SessionId] NVARCHAR(100) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [DeviceType] NVARCHAR(50) NULL,
    [Country] NVARCHAR(100) NULL,
    [City] NVARCHAR(100) NULL,
    [Referrer] NVARCHAR(2000) NULL,
    [ConversionValue] DECIMAL(18,2) NULL,
    [CustomData] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_AdEvents_Campaigns] FOREIGN KEY ([CampaignId]) REFERENCES [Campaigns]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AdEvents_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_AdEvents_TenantId_CampaignId_EventTime] ON [AdEvents] ([TenantId], [CampaignId], [EventTime]);
CREATE INDEX [IX_AdEvents_TenantId_EventType_EventTime] ON [AdEvents] ([TenantId], [EventType], [EventTime]);

PRINT 'Database schema created successfully!';
