using System.Diagnostics;

namespace ConwayClient.Models
{
    public class GameBoard
    {
        private HashSet<CellPosition> _liveCells = new HashSet<CellPosition>();
        public int Rows { get; }
        public int Columns { get; }
        public bool IsUpdated => _updatedCells.Count > 0;
        private List<Cell> _updatedCells = new List<Cell>();
        private CellPool _cellPool = new CellPool();
        private Dictionary<CellPosition, int> _neighborCounts = new Dictionary<CellPosition, int>();
        private HashSet<CellPosition> _newLiveCells = new HashSet<CellPosition>();
        private List<List<Cell>> _history = new List<List<Cell>>();
        private const int MAX_HISTORY_SIZE = 100;

        public GameBoard(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
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

            if (_updatedCells.Any())
            {
                _history.Add(new List<Cell>(_updatedCells));
                if (_history.Count > MAX_HISTORY_SIZE)
                    _history.RemoveAt(0);
            }

            // Swap _liveCells and _newLiveCells
            var temp = _liveCells;
            _liveCells = _newLiveCells;
            _newLiveCells = temp;

            totalStopwatch.Stop();

            Console.WriteLine($"Total time taken: {totalStopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void UndoLastState()
        {
            if (!_history.Any()) return;

            var lastChanges = _history.Last();
            _history.RemoveAt(_history.Count - 1);

            foreach (var cell in lastChanges)
            {
                if (cell.IsAlive)
                {
                    _liveCells.Remove(new CellPosition(cell.Row, cell.Column));
                }
                else
                {
                    _liveCells.Add(new CellPosition(cell.Row, cell.Column));
                }
                _updatedCells.Add(new Cell(cell.Row, cell.Column, !cell.IsAlive));
            }
        }

        public bool CanUndo()
        {
            return _history.Count > 0;
        }

        public bool GetCellState(int row, int col)
        {
            return _liveCells.Contains(new CellPosition(row, col));
        }

        public void SetCellState(int row, int col, bool isAlive)
        {
            var cell = new CellPosition(row, col);

            if (isAlive)
            {
                _liveCells.Add(cell);
            }
            else
            {
                _liveCells.Remove(cell);
            }

            _updatedCells.Add(new Cell(row, col, isAlive));
        }

        private IEnumerable<CellPosition> GetNeighbors(CellPosition cell)
        {
            // Relative positions of the 8 neighbors
            int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newRow = cell.Row + rowOffsets[i];
                int newCol = cell.Col + colOffsets[i];

                if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                {
                    yield return new CellPosition(newRow, newCol);
                }
            }
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
        private const int InitialSize = 4000;  // Adjust as needed

        public CellPool()
        {
            Console.WriteLine("Creating New Object Pool");
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
            Console.WriteLine("Object pool exhausted, creating new CellPosition");
            return new CellPosition(-1, -1);  // Create new if pool is empty
        }

        public void Release(CellPosition cell)
        {
            pool.Push(cell);
        }
    }
}
