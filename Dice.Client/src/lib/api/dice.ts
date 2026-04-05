export async function rollDice(count: number, sides: number): Promise<number[]> {
  const response = await fetch(`/roll/${count}/d/${sides}`);

  if (!response.ok) {
    throw new Error(`Dice API request failed with status ${response.status}`);
  }

  return (await response.json()) as number[];
}
