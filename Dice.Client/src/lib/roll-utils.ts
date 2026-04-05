export function formatTime(date: Date): string {
  const hour = String(date.getHours()).padStart(2, '0');
  const minute = String(date.getMinutes()).padStart(2, '0');
  const second = String(date.getSeconds()).padStart(2, '0');

  return `${hour}:${minute}:${second}`;
}

export function formatResultLabel(count: number, sides: number): string {
  if (sides === 100) {
    return 'd%';
  }

  return `${count}d${sides}`;
}

export function getResultsTotal(results: number[]): number {
  return results.reduce((sum, value) => sum + value, 0);
}

export function getResultsGrid(resultCount: number): { cols: number; rows: number } {
  const cols = Math.min(Math.ceil(Math.sqrt(resultCount)), 10) || 1;
  const rows = Math.ceil(resultCount / cols);

  return { cols, rows };
}

export function formatLiveLogEntry(
  date: Date,
  count: number,
  sides: number,
  results: number[]
): string {
  const timeText = formatTime(date);
  const resultText = formatResultLabel(count, sides);
  const valueText = `[${results.join(', ')}]`;

  return `${timeText} ${resultText} ${valueText}`;
}
