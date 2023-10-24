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

            var liveCellsStopwatch = Stopwatch.StartNew();

            var newLiveCells = new HashSet<CellPosition>();
            _updatedCells.Clear();

            foreach (var cell in _liveCells)
            {
                int liveNeighbors = CountLiveNeighbors(cell);

                if (liveNeighbors == 2 || liveNeighbors == 3)
                {
                    newLiveCells.Add(cell);
                }
                else
                {
                    _updatedCells.Add(new Cell(cell.Row, cell.Col, false));
                }
            }

            liveCellsStopwatch.Stop();

            var neighborsStopwatch = Stopwatch.StartNew();

            var cellsToCheck = _liveCells.SelectMany(cell => GetNeighbors(cell)).Distinct().ToList();
            cellsToCheck.AddRange(_liveCells);

            foreach (var cell in cellsToCheck)
            {
                if (!_liveCells.Contains(cell) && CountLiveNeighbors(cell) == 3)
                {
                    newLiveCells.Add(cell);
                    _updatedCells.Add(new Cell(cell.Row, cell.Col, true));
                }
            }

            neighborsStopwatch.Stop();

            _liveCells = newLiveCells;

            totalStopwatch.Stop();

            Console.WriteLine($"Time taken for live cells processing: {liveCellsStopwatch.Elapsed.TotalMilliseconds} ms");
            Console.WriteLine($"Time taken for neighbors processing: {neighborsStopwatch.Elapsed.TotalMilliseconds} ms");
            Console.WriteLine($"Total time taken: {totalStopwatch.Elapsed.TotalMilliseconds} ms");
        }

        private int CountLiveNeighbors(CellPosition cell)
        {
            int count = 0;

            for (int row = -1; row <= 1; row++)
            {
                for (int col = -1; col <= 1; col++)
                {
                    if (row == 0 && col == 0) continue;

                    int newRow = cell.Row + row;
                    int newCol = cell.Col + col;

                    if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns &&
                        _liveCells.Contains(new CellPosition(newRow, newCol)))
                    {
                        count++;
                    }
                }
            }

            return count;
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
}
