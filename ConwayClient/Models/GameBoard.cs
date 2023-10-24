using System.Collections.Concurrent;
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
        private CellPool _cellPool = new CellPool();
        private Dictionary<CellPosition, int> _neighborCounts = new Dictionary<CellPosition, int>();
        private HashSet<CellPosition> _newLiveCells = new HashSet<CellPosition>();

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
            var totalStopwatch = Stopwatch.StartNew();

            // Clear the dictionary for reuse
            _neighborCounts.Clear();

            // Clear the HashSet for reuse
            _newLiveCells.Clear();

            // Efficiently count neighbors using the dictionary
            foreach (var cell in _liveCells)
            {
                foreach (var neighbor in GetNeighbors(cell))
                {
                    if (_neighborCounts.ContainsKey(neighbor))
                    {
                        _neighborCounts[neighbor]++;
                    }
                    else
                    {
                        _neighborCounts[neighbor] = 1;
                    }
                }
            }

            // Explicitly process each live cell
            foreach (var cell in _liveCells)
            {
                var count = _neighborCounts.ContainsKey(cell) ? _neighborCounts[cell] : 0;

                if (count == 2 || count == 3)
                {
                    _newLiveCells.Add(cell);
                }
                else
                {
                    _updatedCells.Add(new Cell(cell.Row, cell.Col, false));
                    _cellPool.Release(cell);
                }
            }

            // Process potential new live cells (cells that were dead but have 3 neighbors)
            foreach (var entry in _neighborCounts)
            {
                var cell = entry.Key;
                var count = entry.Value;

                if (!_liveCells.Contains(cell) && count == 3)
                {
                    _newLiveCells.Add(cell);
                    _updatedCells.Add(new Cell(cell.Row, cell.Col, true));
                }
                else
                {
                    _cellPool.Release(cell);
                }
            }

            // Swap _liveCells and _newLiveCells
            var temp = _liveCells;
            _liveCells = _newLiveCells;
            _newLiveCells = temp;

            totalStopwatch.Stop();

            Console.WriteLine($"Total time taken: {totalStopwatch.Elapsed.TotalMilliseconds} ms");
        }

        private IEnumerable<CellPosition> GetNeighbors(CellPosition cell)
        {
            for (int row = -1; row <= 1; row++)
            {
                for (int col = -1; col <= 1; col++)
                {
                    if (row == 0 && col == 0) continue;  // skip the cell itself

                    int newRow = cell.Row + row;
                    int newCol = cell.Col + col;

                    if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                    {
                        yield return new CellPosition(newRow, newCol);
                    }
                }
            }
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
                    bool isAlive = rand.Next(100) < 20;  // 20% chance for a cell to be alive
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

    public class CellPool
    {
        private Stack<CellPosition> pool = new Stack<CellPosition>();
        private const int InitialSize = 1000;  // Adjust as needed

        public CellPool()
        {
            for (int i = 0; i < InitialSize; i++)
            {
                pool.Push(new CellPosition(-1, -1));
            }
        }

        public CellPosition Acquire()
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
            return new CellPosition(-1, -1);  // Create new if pool is empty
        }

        public void Release(CellPosition cell)
        {
            pool.Push(cell);
        }
    }
}
