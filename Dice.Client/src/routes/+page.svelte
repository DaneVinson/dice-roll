<svelte:head>
  <title>Dice Roller</title>
  <meta name="description" content="SvelteKit client for Dice.Api" />
</svelte:head>

<script lang="ts">
  import { rollDice } from '$lib/api/dice';
  import {
    formatLiveLogEntry,
    formatResultLabel,
    getResultsGrid,
    getResultsTotal
  } from '$lib/roll-utils';

  const diceSides = [4, 6, 8, 10, 12, 20];
  const quickCounts = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

  let selectedCounts: Record<number, number> = {
    4: 1,
    6: 1,
    8: 1,
    10: 1,
    12: 1,
    20: 1
  };

  let lastRollCount = 0;
  let lastRollSides = 0;
  let results: number[] = [];
  let liveLog: string[] = [];
  let isLoading = false;
  let error = '';
  let resultCols = 1;
  let resultRows = 0;

  $: lastRollLabel = formatResultLabel(lastRollCount, lastRollSides);
  $: lastRollTotal = getResultsTotal(results);
  $: ({ cols: resultCols, rows: resultRows } = getResultsGrid(results.length));

  async function rollForSides(count: number, sides: number) {
    if (isLoading) {
      return;
    }

    isLoading = true;
    error = '';

    try {
      const fetchedResults = await rollDice(count, sides);
      const logEntry = formatLiveLogEntry(new Date(), count, sides, fetchedResults);

      results = fetchedResults;
      lastRollCount = count;
      lastRollSides = sides;
      liveLog = [logEntry, ...liveLog];
      selectedCounts = {
        ...selectedCounts,
        [sides]: 1
      };
    } catch (err) {
      error = err instanceof Error ? err.message : 'Unknown error calling Dice API.';
    } finally {
      isLoading = false;
    }
  }

  function handleCountChange(sides: number, event: Event) {
    const select = event.currentTarget as HTMLSelectElement;
    const count = Number.parseInt(select.value, 10);

    if (Number.isNaN(count)) {
      return;
    }

    selectedCounts = {
      ...selectedCounts,
      [sides]: count
    };

    void rollForSides(count, sides);
  }

  function handleCountClick(sides: number) {
    void rollForSides(selectedCounts[sides], sides);
  }

  function handleD100Click() {
    void rollForSides(1, 100);
  }
</script>

<main>
  <h1>Dice Roller</h1>

  <section class="controls" aria-label="Dice controls">
    {#each diceSides as sides}
      <label>
        <div class="split-control">
          <button
            type="button"
            on:click={() => handleCountClick(sides)}
            disabled={isLoading}
            aria-label={`Roll d${sides} with count ${selectedCounts[sides]}`}
          >
            {sides}
          </button>
          <select
            value={selectedCounts[sides]}
            on:change={(event) => handleCountChange(sides, event)}
            disabled={isLoading}
            aria-label={`Select d${sides} roll count`}
          >
            {#each quickCounts as value}
              <option value={value}>{value}</option>
            {/each}
          </select>
        </div>
      </label>
    {/each}

    <label class="d100-control">
      <button
        type="button"
        on:click={handleD100Click}
        disabled={isLoading}
        aria-label="Roll d100"
      >
        d%
      </button>
    </label>
  </section>

  {#if error}
    <p class="error">{error}</p>
  {/if}

  {#if results.length > 0}
    <section>
      <h2>
        {lastRollLabel}{#if lastRollCount > 1}{' '}(Total: {lastRollTotal}){/if}
      </h2>
      <table class="results-table">
        <tbody>
          {#each Array(resultRows) as _, rowIdx}
            <tr>
              {#each Array(resultCols) as _, colIdx}
                {@const resultIdx = rowIdx * resultCols + colIdx}
                <td>
                  {#if resultIdx < results.length}
                    {results[resultIdx]}
                  {/if}
                </td>
              {/each}
            </tr>
          {/each}
        </tbody>
      </table>
    </section>
  {/if}

  {#if liveLog.length > 0}
    <section>
      <ul class="live-log-list">
        {#each liveLog as entry}
          <li>{entry}</li>
        {/each}
      </ul>
    </section>
  {/if}
</main>

<style>
  :global(body) {
    margin: 0;
    font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
    background: radial-gradient(circle at top left, #1f2937, #0b1120 60%);
    color: #e5e7eb;
  }

  main {
    max-width: 42rem;
    margin: 10vh auto;
    padding: 2rem;
    border-radius: 1rem;
    background: rgba(15, 23, 42, 0.88);
    border: 1px solid #334155;
    box-shadow: 0 16px 40px rgba(2, 6, 23, 0.5);
  }

  h1 {
    margin-top: 0;
    font-size: 2rem;
  }

  .controls {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(3.5rem, 1fr));
    gap: 0.75rem;
    align-items: stretch;
    margin-top: 1rem;
  }

  label {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    font-weight: 600;
    align-items: flex-start;
  }

  select,
  button {
    font: inherit;
    padding: 0.6rem 0.7rem;
    border-radius: 0.55rem;
    border: 1px solid #22c55e;
    background: #22c55e;
    color: #052e16;
    font-weight: 700;
  }

  select,
  button {
    transition: filter 120ms ease, transform 120ms ease, box-shadow 120ms ease;
  }

  select:hover:not(:disabled),
  button:hover:not(:disabled) {
    filter: brightness(1.04);
    transform: translateY(-1px);
  }

  select:focus-visible,
  button:focus-visible {
    outline: none;
    box-shadow: 0 0 0 2px #0f172a, 0 0 0 4px #86efac;
  }

  button {
    background: #22c55e;
    color: #052e16;
    border-color: #22c55e;
    cursor: pointer;
    font-weight: 700;
  }

  label:last-child button {
    width: 2.5rem;
    min-width: 2.5rem;
    min-height: 2.5rem;
  }

  .split-control {
    display: grid;
    grid-template-columns: 1fr auto;
    gap: 0.4rem;
    align-items: center;
  }

  .split-control button {
    width: 2.5rem;
    min-width: 2.5rem;
    min-height: 2.5rem;
  }

  .split-control select {
    width: 1.5rem;
    min-width: 1.5rem;
    padding-left: 0.2rem;
    padding-right: 0.2rem;
    appearance: none;
    -webkit-appearance: none;
    -moz-appearance: none;
    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3E%3Cpath fill='%23052e16' d='M4.47 5.97a.75.75 0 0 1 1.06 0L8 8.44l2.47-2.47a.75.75 0 1 1 1.06 1.06l-3 3a.75.75 0 0 1-1.06 0l-3-3a.75.75 0 0 1 0-1.06Z'/%3E%3C/svg%3E");
    background-repeat: no-repeat;
    background-position: center;
    background-size: 1rem;
    color: transparent;
    text-shadow: 0 0 0 transparent;
  }

  .split-control select option {
    color: #052e16;
  }

  button:disabled {
    opacity: 0.65;
    cursor: default;
  }

  .error {
    margin-top: 1rem;
    color: #fca5a5;
    font-weight: 600;
  }

  section {
    margin-top: 1.5rem;
    padding-top: 1rem;
    border-top: 1px solid #334155;
  }

  .results-table {
    width: auto;
    border-collapse: collapse;
    margin: 0.75rem 0;
  }

  .results-table td {
    width: 2.75rem;
    height: 2.75rem;
    text-align: center;
    vertical-align: middle;
    padding: 0;
    border: 1px solid #334155;
    font-weight: 600;
  }

  .results-table td:empty {
    border: 1px solid transparent;
  }

  .live-log-list {
    margin: 0.75rem 0 0;
    padding: 0;
    list-style: none;
    font-family: Consolas, "Courier New", monospace;
    font-size: 0.95rem;
  }

  .live-log-list li {
    padding: 0.2rem 0;
    white-space: nowrap;
    overflow-x: auto;
  }

  p {
    line-height: 1.6;
  }

  @media (max-width: 760px) {
    main {
      margin: 0;
      min-height: 100vh;
      border-radius: 0;
    }

    .controls {
      grid-template-columns: repeat(3, minmax(0, 1fr));
    }
  }
</style>