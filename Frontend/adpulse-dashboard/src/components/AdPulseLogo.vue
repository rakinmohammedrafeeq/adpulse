<template>
  <div class="adpulse-logo-container" :class="[size, theme]">
    <div class="logo-mark">
      <svg
        viewBox="0 0 48 48"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        class="logo-svg"
      >
        <defs>
          <linearGradient id="logoGrad" x1="0%" y1="0%" x2="100%" y2="100%">
            <stop offset="0%" stop-color="#06b6d4" />
            <stop offset="50%" stop-color="#3b82f6" />
            <stop offset="100%" stop-color="#8b5cf6" />
          </linearGradient>
          <linearGradient id="pulseGlow" x1="0%" y1="0%" x2="100%" y2="0%">
            <stop offset="0%" stop-color="#06b6d4" stop-opacity="0.8" />
            <stop offset="100%" stop-color="#3b82f6" stop-opacity="0.8" />
          </linearGradient>
        </defs>

        <!-- Rounded Hexagon Base Plate -->
        <rect
          x="3"
          y="3"
          width="42"
          height="42"
          rx="12"
          fill="url(#logoGrad)"
          class="base-rect"
        />

        <!-- Concentric Radar / Targeting Rings -->
        <circle cx="24" cy="24" r="14" stroke="white" stroke-opacity="0.25" stroke-width="1.5" stroke-dasharray="2 3" />
        <circle cx="24" cy="24" r="8" stroke="white" stroke-opacity="0.35" stroke-width="1.5" />

        <!-- Dynamic Pulse Waveform (Electrocardiogram / Ad Impulse) -->
        <path
          d="M10 24H16L19 18L23 31L27 15L30 27L33 24H38"
          stroke="white"
          stroke-width="2.5"
          stroke-linecap="round"
          stroke-linejoin="round"
        />

        <!-- Signal Core Point -->
        <circle cx="27" cy="15" r="2" fill="#38bdf8" />
      </svg>
    </div>

    <div v-if="showText" class="logo-text">
      <span class="brand-title">Ad<span class="pulse-highlight">Pulse</span></span>
      <span v-if="tagline" class="brand-tagline">Campaign Intelligence</span>
    </div>
  </div>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    size?: 'small' | 'medium' | 'large';
    theme?: 'light' | 'dark';
    showText?: boolean;
    tagline?: boolean;
  }>(),
  {
    size: 'medium',
    theme: 'dark',
    showText: true,
    tagline: false
  }
);
</script>

<style scoped>
.adpulse-logo-container {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  user-select: none;
  font-family: -apple-system, BlinkMacSystemFont, 'Inter', 'Segoe UI', Roboto, sans-serif;
}

.logo-mark {
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  transition: transform 0.25s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.adpulse-logo-container:hover .logo-mark {
  transform: scale(1.08) rotate(-2deg);
}

.logo-svg {
  filter: drop-shadow(0 4px 12px rgba(59, 130, 246, 0.35));
}

/* Sizes */
.small .logo-svg {
  width: 28px;
  height: 28px;
}
.small .brand-title {
  font-size: 16px;
}

.medium .logo-svg {
  width: 36px;
  height: 36px;
}
.medium .brand-title {
  font-size: 20px;
}

.large .logo-svg {
  width: 52px;
  height: 52px;
}
.large .brand-title {
  font-size: 28px;
}

/* Typography */
.logo-text {
  display: flex;
  flex-direction: column;
  line-height: 1;
}

.brand-title {
  font-weight: 800;
  letter-spacing: -0.6px;
  transition: color 0.2s ease;
}

.dark .brand-title {
  color: #f8fafc;
}

.light .brand-title {
  color: #0f172a;
}

.pulse-highlight {
  background: linear-gradient(135deg, #38bdf8 0%, #818cf8 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.brand-tagline {
  font-size: 10px;
  font-weight: 600;
  letter-spacing: 0.8px;
  text-transform: uppercase;
  color: #94a3b8;
  margin-top: 2px;
}
</style>
