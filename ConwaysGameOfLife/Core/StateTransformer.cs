using ConwaysGameOfLife.Entities;
using ConwaysGameOfLife.Utils;
using System.Diagnostics;

namespace ConwaysGameOfLife.Core
{
    public class StateTransformer
    {
        private Dictionary<CellPosition, int> _neighborCounts = new Dictionary<CellPosition, int>();
        private HashSet<CellPosition> _newLiveCells = new HashSet<CellPosition>();

        public GameState GenerateNextState(GameState currentState)
        {
            var totalStopwatch = Stopwatch.StartNew();

            // Clear the dictionary for reuse
            _neighborCounts.Clear();

            // Clear the HashSet for reuse
            _newLiveCells.Clear();

            // Efficiently count neighbors using the dictionary
            foreach (var cell in currentState.GetLiveCells())
            {
                foreach (var neighbor in NeighborCalculator.GetNeighbors(cell, currentState))
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
            foreach (var cell in currentState.GetLiveCells())
            {
                var count = _neighborCounts.ContainsKey(cell) ? _neighborCounts[cell] : 0;

                if (count == 2 || count == 3)
                {
                    _newLiveCells.Add(cell);
                }
            }

            // Process potential new live cells (cells that were dead but have 3 neighbors)
            foreach (var entry in _neighborCounts)
            {
                var cell = entry.Key;
                var count = entry.Value;

                if (!currentState.IsCellAlive(cell.Row, cell.Col) && count == 3)
                {
                    _newLiveCells.Add(cell);
                }
            }

            // Construct the new GameState based on _newLiveCells
            GameState nextState = new GameState(currentState.Rows, currentState.Columns);
            foreach (var cell in _newLiveCells)
            {
                nextState.AddLiveCell(cell.Row, cell.Col);
            }

            totalStopwatch.Stop();
            Console.WriteLine($"(StateTransformer): Total time taken: {totalStopwatch.Elapsed.TotalMilliseconds} ms");

            return nextState;
        }

        public GameState CreateStateFromCellPositions(int rows, int columns, HashSet<CellPosition> liveCells)
        {
            GameState newState = new GameState(rows, columns);

            foreach (var cell in liveCells)
            {
                newState.AddLiveCell(cell.Row, cell.Col);
            }

            return newState;
        }
    }
}
