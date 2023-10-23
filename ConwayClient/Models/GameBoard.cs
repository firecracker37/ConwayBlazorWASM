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

        private int GetAliveNeighborsCount(Cell[,] state, int x, int y)
        {
            int aliveNeighbors = 0;
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (newX >= 0 && newX < Rows && newY >= 0 && newY < Columns && state[newX, newY].IsAlive)
                {
                    aliveNeighbors++;
                    if (aliveNeighbors == 4)  // Optimization: further counting is unnecessary
                    {
                        return aliveNeighbors;
                    }
                }
            }

            return aliveNeighbors;
        }

        public void ApplyStateChange(Action<Cell[,]> stateChangeAction)
        {
            // Clone the current state and push it to the stack to maintain history
            Cell[,] currentState = CloneCurrentState();
            _previousStates.Push(currentState);

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Apply the state change
            stateChangeAction(currentState);

            stopwatch.Stop();
            Console.WriteLine($"Game state updated in {stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        private Cell[,] CloneCurrentState()
        {
            Cell[,] clone = new Cell[Rows, Columns];
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    // Clone each cell object or just copy the state, depending on your specific needs
                    clone[x, y] = new Cell(x, y, Cells[x, y].IsAlive);
                }
            }
            return clone;
        }

        public void GenerateNextState()
        {
            ApplyStateChange(currentState =>
            {
                Parallel.For(0, Rows, x =>
                {
                    for (int y = 0; y < Columns; y++)
                    {
                        var cell = currentState[x, y];
                        var aliveNeighbors = GetAliveNeighborsCount(currentState, x, y);

                        if (cell.IsAlive)
                        {
                            if (aliveNeighbors < 2 || aliveNeighbors > 3)
                            {
                                Cells[x, y].IsAlive = false; // Cell dies
                            }
                        }
                        else
                        {
                            if (aliveNeighbors == 3)
                            {
                                Cells[x, y].IsAlive = true; // Cell becomes alive
                            }
                        }
                    }
                });
            });
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

        public void ResetGame()
        {
            ApplyStateChange(currentState =>
            {
                Parallel.For(0, Rows, x =>
                {
                    for (int y = 0; y < Columns; y++)
                    {
                        Cells[x, y].IsAlive = false; // Reset cells in place
                    }
                });
            });

            _previousStates.Clear(); // Clear the history after resetting the game
        }

        public void RandomizeBoard()
        {
            ApplyStateChange(currentState =>
            {
                Random random = new Random();

                for (int x = 0; x < Rows; x++)
                {
                    for (int y = 0; y < Columns; y++)
                    {
                        Cells[x, y].IsAlive = random.Next(2) == 0;
                    }
                }
            });
        }

        public void ToggleCellState(int x, int y)
        {
            ApplyStateChange(currentState =>
            {
                if (x >= 0 && x < Rows && y >= 0 && y < Columns)
                {
                    Cells[x, y].IsAlive = !Cells[x, y].IsAlive;
                }
            });
        }
    }
}
