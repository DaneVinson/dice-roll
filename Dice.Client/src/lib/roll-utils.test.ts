import { describe, expect, it } from 'vitest';

import {
  formatLiveLogEntry,
  formatResultLabel,
  formatTime,
  getResultsGrid,
  getResultsTotal
} from './roll-utils';

describe('roll-utils', () => {
  it('formats time as hh:mm:ss', () => {
    const time = formatTime(new Date(2026, 3, 4, 9, 5, 7));

    expect(time).toBe('09:05:07');
  });

  it('formats d100 label as d%', () => {
    expect(formatResultLabel(1, 100)).toBe('d%');
  });

  it('formats normal roll labels as count d sides', () => {
    expect(formatResultLabel(3, 8)).toBe('3d8');
  });

  it('calculates roll total from results', () => {
    expect(getResultsTotal([4, 6, 2])).toBe(12);
  });

  it('returns a square-ish grid capped at 10 columns', () => {
    expect(getResultsGrid(7)).toEqual({ cols: 3, rows: 3 });
    expect(getResultsGrid(150)).toEqual({ cols: 10, rows: 15 });
    expect(getResultsGrid(0)).toEqual({ cols: 1, rows: 0 });
  });

  it('formats live log entries using the display rules', () => {
    const entry = formatLiveLogEntry(new Date(2026, 3, 4, 21, 15, 9), 2, 6, [1, 5]);

    expect(entry).toBe('21:15:09 2d6 [1, 5]');
  });
});
