using System.Diagnostics;

namespace ConwayClient.Models
{
    public class GameBoard
    {
        private Stack<Cell[,]> _previousStates = new Stack<Cell[,]>();
        public Cell[,] Cells { get; private set; }
        public int Rows { get; }
        public int Columns { get; }
        public bool IsRunning { get; private set; }

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

        public Cell[,] GenerateNextState()
        {
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
                        }
                    }
                    else
                    {
                        if (aliveNeighbors == 3)
                        {
                            nextGameState[x, y].IsAlive = true; // Cell becomes alive
                        }
                    }
                }
            }

            return nextGameState;
        }

        public Cell[,] GetPreviousState()
        {
            if (_previousStates.Count > 0)
            {
                return _previousStates.Pop();  // Retrieve the last stored state
            }
            else
            {
                // Handle the case where there are no more stored states
                // Perhaps return the current state, or handle this situation in another appropriate way for your application
                return Cells;
            }
        }

        public void UpdateGameState(Cell[,] nextState)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _previousStates.Push(Cells);

            try
            {
                Cells = nextState;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Game state updated in {stopwatch.Elapsed.TotalMilliseconds} ms");
            }
        }

        public void ResetGame()
        {
            Cell[,] resetState = new Cell[Rows, Columns];

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    resetState[x, y] = new Cell(x, y, false);
                }
            }

            UpdateGameState(resetState);
            _previousStates.Clear();
        }

        public void RandomizeBoard()
        {
            Cell[,] randomState = new Cell[Rows, Columns];
            Random random = new Random();

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    randomState[x, y] = new Cell(x, y, random.Next(2) == 0);
                }
            }

            UpdateGameState(randomState);
        }
    }
}
