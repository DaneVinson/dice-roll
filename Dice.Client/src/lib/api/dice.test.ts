import { afterEach, describe, expect, it, vi } from 'vitest';

import { rollDice } from './dice';

describe('rollDice', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('returns parsed dice values for a successful response', async () => {
    const mockJson = vi.fn().mockResolvedValue([3, 4, 6]);
    const mockFetch = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: mockJson
    });

    vi.stubGlobal('fetch', mockFetch);

    const result = await rollDice(3, 6);

    expect(mockFetch).toHaveBeenCalledWith('/roll/3/d/6');
    expect(result).toEqual([3, 4, 6]);
  });

  it('throws an error when the API response is not successful', async () => {
    const mockFetch = vi.fn().mockResolvedValue({
      ok: false,
      status: 500,
      json: vi.fn()
    });

    vi.stubGlobal('fetch', mockFetch);

    await expect(rollDice(2, 10)).rejects.toThrow(
      'Dice API request failed with status 500'
    );
  });
});
