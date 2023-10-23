using System.Diagnostics;

namespace ConwayClient.Models
{
    public class GameBoard
    {
        private HashSet<CellPosition> _liveCells = new HashSet<CellPosition>();
        public int Rows { get; }
        public int Columns { get; }
        public bool IsRunning { get; private set; }
        public bool IsUpdated => _updatedCells.Count > 0;
        private List<Cell> _updatedCells = new List<Cell>();

        public GameBoard(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            IsRunning = false;
        }

        public void StartGame()
        {
            IsRunning = true;
        }

        public void PauseGame()
        {
            IsRunning = false;
        }

        public List<Cell> GetUpdatedCells() => _updatedCells;

        public void ResetIsUpdated() => _updatedCells.Clear();

        public void GenerateNextState()
        {
            var stopwatch = Stopwatch.StartNew();  // Start the stopwatch

            var newLiveCells = new HashSet<CellPosition>();
            var neighborsToCheck = new HashSet<CellPosition>();
            _updatedCells.Clear();

            foreach (var cell in _liveCells)
            {
                int liveNeighbors = CountLiveNeighbors(cell);
                neighborsToCheck.UnionWith(GetNeighbors(cell));

                if (liveNeighbors == 2 || liveNeighbors == 3)
                {
                    newLiveCells.Add(cell);
                }
                else
                {
                    _updatedCells.Add(new Cell(cell.Row, cell.Col, false));  // Add dead cells to the updated cells list
                }
            }

            foreach (var neighbor in neighborsToCheck)
            {
                if (!_liveCells.Contains(neighbor) && CountLiveNeighbors(neighbor) == 3)
                {
                    newLiveCells.Add(neighbor);
                    _updatedCells.Add(new Cell(neighbor.Row, neighbor.Col, true));
                }
            }

            _liveCells = newLiveCells;

            stopwatch.Stop();  // Stop the stopwatch
            var elapsedTime = stopwatch.Elapsed;  // Get the elapsed time

            Console.WriteLine($"Time taken to generate next state: {elapsedTime.TotalMilliseconds} ms");  // Print the time to the console
        }


        private int CountLiveNeighbors(CellPosition cell)
        {
            return GetNeighbors(cell).Count(neighbor => _liveCells.Contains(neighbor));
        }

        private IEnumerable<CellPosition> GetNeighbors(CellPosition cell)
        {
            var neighbors = new List<CellPosition>();

            for (int row = -1; row <= 1; row++)
            {
                for (int col = -1; col <= 1; col++)
                {
                    if (row == 0 && col == 0) continue; // skip the cell itself

                    int newRow = cell.Row + row;
                    int newCol = cell.Col + col;

                    if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                    {
                        neighbors.Add(new CellPosition(newRow, newCol));
                    }
                }
            }

            return neighbors;
        }

        public bool IsCellAlive(int row, int col)
        {
            return _liveCells.Contains(new CellPosition(row, col));
        }

        public void ResetGame()
        {
            _updatedCells.Clear();

            foreach (var cell in _liveCells)
            {
                _updatedCells.Add(new Cell(cell.Row, cell.Col, false)); 
            }

            _liveCells.Clear();
        }

        public void RandomizeBoard()
        {
            _updatedCells.Clear();
            var rand = new Random();
            var newLiveCells = new HashSet<CellPosition>();

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    bool isAlive = rand.Next(2) == 1;
                    var cell = new CellPosition(row, col);

                    if (isAlive)
                    {
                        newLiveCells.Add(cell);
                    }

                    if (isAlive != _liveCells.Contains(cell))  // If the state is changed
                    {
                        _updatedCells.Add(new Cell(row, col, isAlive));
                    }
                }
            }

            _liveCells = newLiveCells;
        }

        public void ToggleCellState(int row, int col)
        {
            var cell = new CellPosition(row, col);
            bool isAlive;

            if (!_liveCells.Remove(cell))
            {
                _liveCells.Add(cell);
                isAlive = true;
            }
            else
            {
                isAlive = false;
            }

            _updatedCells.Add(new Cell(row, col, isAlive));
        }
    }

    public struct CellPosition
    {
        public int Row { get; }
        public int Col { get; }

        public CellPosition(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public override bool Equals(object obj)
        {
            if (obj is CellPosition other)
            {
                return Row == other.Row && Col == other.Col;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }
    }
}
