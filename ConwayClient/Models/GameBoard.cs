namespace ConwayClient.Models
{
    public class GameBoard
    {
        private Timer _gameTimer;
        public Cell[,] Cells { get; private set; }
        public int Rows { get; }
        public int Columns { get; }
        public bool IsRunning { get; private set; }
        public bool IsUpdating { get; private set; }
        public event Action OnGameBoardUpdated;

        public GameBoard(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Cells = new Cell[rows, columns];
            IsRunning = false;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Cells[y, x] = new Cell(x, y, false);  // Initially all cells are dead
                }
            }
        }

        public void StartGame()
        {
            if (IsRunning) return;  // Avoid starting the game if it's already running

            IsRunning = true;

            _gameTimer = new Timer(async _ =>
            {
                if (!IsUpdating)
                {
                    await UpdateGameStateAsync();
                }
            },
            null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));  // Adjust the interval as needed
        }

        public void StopGame()
        {
            if (!IsRunning) return;  // Avoid stopping the game if it's not running

            _gameTimer?.Dispose();
            _gameTimer = null;

            IsRunning = false;
        }

        public List<Cell> GetNeighbors(int x, int y)
        {
            var neighbors = new List<Cell>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;  // Skip the current cell
                    if (x + i < 0 || x + i >= Cells.GetLength(0)) continue;  // Check bounds
                    if (y + j < 0 || y + j >= Cells.GetLength(1)) continue;  // Check bounds

                    neighbors.Add(Cells[x + i, y + j]);
                }
            }

            return neighbors;
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
                                nextGameState[x, y].IsAlive = false; // Cell dies
                                changedCells.Add(nextGameState[x, y]); // Add to changed cells list
                            }
                        }
                        else
                        {
                            if (aliveNeighbors == 3)
                            {
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

    }
}
