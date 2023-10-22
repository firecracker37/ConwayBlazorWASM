namespace ConwayClient.Models
{
    public class GameBoard
    {
        public Cell[,] Cells { get; private set; }
        public int Rows { get; }
        public int Columns { get; }
        public bool IsRunning { get; private set; }
        public bool IsUpdating { get; private set; }
        private SemaphoreSlim _updateSemaphore = new SemaphoreSlim(1, 1);


        public GameBoard(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Cells = new Cell[rows, columns];
            IsRunning = false;

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    Cells[x, y] = new Cell(x, y, false);  // Initially all cells are dead
                }
            }
        }

        public void StartGame()
        {
            IsRunning = true;
        }

        public void PauseGame()
        {
            IsRunning = false;
        }

        public async Task StartUpdateAsync()
        {
            await _updateSemaphore.WaitAsync();
        }

        public void FinishUpdate()
        {
            _updateSemaphore.Release();
        }

        public int GetAliveNeighborsCount(int x, int y)
        {
            int aliveNeighbors = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;  // Skip the current cell
                    if (x + i < 0 || x + i >= Rows) continue;  // Check bounds
                    if (y + j < 0 || y + j >= Columns) continue;  // Check bounds

                    if (Cells[x + i, y + j].IsAlive)
                    {
                        aliveNeighbors++;
                    }
                }
            }

            return aliveNeighbors;
        }

        public async Task<List<Cell>> UpdateGameStateAsync()
        {
            await StartUpdateAsync();  // Wait for any ongoing updates to finish

            try
            {
                return await Task.Run(() =>
                {
                    IsUpdating = true;

                    var changedCells = new List<Cell>();
                    Cell[,] nextGameState = new Cell[Rows, Columns];

                    for (int x = 0; x < Rows; x++)
                    {
                        for (int y = 0; y < Columns; y++)
                        {
                            var cell = Cells[x, y];
                            var aliveNeighbors = GetAliveNeighborsCount(x, y);

                            nextGameState[x, y] = new Cell(x, y, cell.IsAlive); // Copy the current state to the next state

                            if (cell.IsAlive)
                            {
                                if (aliveNeighbors < 2 || aliveNeighbors > 3)
                                {
                                    Console.WriteLine($"Updating Cell at ({cell.X}, {cell.Y}) to {cell.IsAlive}");
                                    nextGameState[x, y].IsAlive = false; // Cell dies
                                    changedCells.Add(nextGameState[x, y]); // Add to changed cells list
                                }
                            }
                            else
                            {
                                if (aliveNeighbors == 3)
                                {
                                    Console.WriteLine($"Updating Cell at ({cell.X}, {cell.Y}) to {cell.IsAlive}");
                                    nextGameState[x, y].IsAlive = true; // Cell becomes alive
                                    changedCells.Add(nextGameState[x, y]); // Add to changed cells list
                                }
                            }
                        }
                    }

                    Cells = nextGameState; // Update the game state in a batch
                    IsUpdating = false;

                    return changedCells; // Return the list of cells that changed state
                });
            }
            finally
            {
                FinishUpdate();  // Release the semaphore after the update
            }
        }

        public void ResetGame()
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Cells[x, y].IsAlive = false;
                }
            }
        }

        public void RandomizeBoard()
        {
            ResetGame();
            Random random = new Random();

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Cells[x, y].IsAlive = random.Next(2) == 0;  // This will randomly set the cell to be alive or dead
                }
            }
        }

    }
}
