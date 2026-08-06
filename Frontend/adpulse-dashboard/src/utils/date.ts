/**
 * AdPulse Date & Time Utilities
 * Provides timezone-aware date parsing and formatting anchored to Indian Standard Time (IST / Asia/Kolkata, UTC+5:30).
 */

export function parseUtcDate(dateInput: string | Date | null | undefined): Date {
  if (!dateInput) return new Date();
  if (dateInput instanceof Date) return dateInput;

  let str = String(dateInput).trim();
  // If the ISO string lacks 'Z' and offset, enforce UTC 'Z' so local browsers do not misinterpret UTC as local
  if (!str.endsWith('Z') && !/[+-]\d{2}:\d{2}$/.test(str)) {
    str += 'Z';
  }
  const d = new Date(str);
  return isNaN(d.getTime()) ? new Date() : d;
}

/**
 * Format as date in Indian Standard Time (IST)
 * Example: "17 Sep 2026"
 */
export function formatDate(dateInput: string | Date | null | undefined): string {
  if (!dateInput) return '';
  const d = parseUtcDate(dateInput);

  return new Intl.DateTimeFormat('en-IN', {
    timeZone: 'Asia/Kolkata',
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(d);
}

/**
 * Format as time and date in Indian Standard Time (IST)
 * Example: "02:15:30 am · 17 Sep 2026"
 */
export function formatTime(dateInput: string | Date | null | undefined): string {
  if (!dateInput) return '';
  const d = parseUtcDate(dateInput);

  const timePart = new Intl.DateTimeFormat('en-IN', {
    timeZone: 'Asia/Kolkata',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: true
  }).format(d);

  const datePart = new Intl.DateTimeFormat('en-IN', {
    timeZone: 'Asia/Kolkata',
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(d);

  return `${timePart} · ${datePart}`;
}
