using ConwaysGameOfLife.Core;
using ConwaysGameOfLife.Entities;
using ConwaysGameOfLife.History;
using System.Diagnostics;

namespace ConwayClient.Models
{
    public class GameBoard
    {
        private object _stateLock = new object();
        public int Rows => _currentState.Rows;
        public int Columns => _currentState.Columns;

        private GameState _currentState;
        private StateTransformer _stateTransformer = new StateTransformer();
        private HistoryManager _historyManager = new HistoryManager();
        private List<CellPosition> _bornCells;
        private List<CellPosition> _deadCells;
        private bool _stateUpdated = false;

        public List<CellPosition> GetBornCells() => _bornCells;
        public List<CellPosition> GetDeadCells() => _deadCells;

        public GameBoard(int rows, int columns)
        {
            _currentState = new GameState(rows, columns);
            _bornCells = new List<CellPosition>();
            _deadCells = new List<CellPosition>();
        }

        public void UpdateState(GameState newState, bool addToHistory = true)
        {
            // Determine the born and dead cells by comparing _currentState and newState
            _bornCells.Clear();
            _deadCells.Clear();

            foreach (var cell in newState.GetLiveCells())
            {
                if (!_currentState.IsCellAlive(cell.Row, cell.Col))
                {
                    _bornCells.Add(cell);
                }
            }
            foreach (var cell in _currentState.GetLiveCells())
            {
                if (!newState.IsCellAlive(cell.Row, cell.Col))
                {
                    _deadCells.Add(cell);
                }
            }

            // Save the old state to the history before updating to the new one
            if (addToHistory)
            {
                _historyManager.AddToHistory(_currentState);
            }

            _currentState = newState;  // Update the current state to the new state
            _stateUpdated = true;      // Indicate that the state has been updated
        }

        public bool StateUpdated
        {
            get { return _stateUpdated; }
        }

        public void ResetStateUpdatedFlag()
        {
            _stateUpdated = false;
        }

        public void GenerateNextState()
        {
            lock (_stateLock)
            {
                var totalStopwatch = Stopwatch.StartNew();

                // Use StateTransformer to get the new state
                var newState = _stateTransformer.GenerateNextState(_currentState);

                UpdateState(newState);
            }
        }

        public void UndoLastState()
        {
            lock (_stateLock)
            {
                if (_historyManager.IsHistoryEmpty) return;
                var lastState = _historyManager.UndoLastState();

                UpdateState(lastState, false);
            }
        }

        public bool CanUndo()
        {
            return !_historyManager.IsHistoryEmpty;
        }


        public bool GetCellState(int row, int col)
        {
            return _currentState.IsCellAlive(row, col);
        }

        public void SetCellState(int row, int col, bool isAlive)
        {
            var newState = _currentState.Clone();

            if (isAlive)
            {
                newState.AddLiveCell(row, col);
            }
            else
            {
                newState.RemoveLiveCell(row, col);
            }

            UpdateState(newState);
        }

        public void ToggleCellState(int row, int col)
        {
            var newState = _currentState.Clone();

            if (_currentState.IsCellAlive(row, col))
            {
                newState.RemoveLiveCell(row, col);
            }
            else
            {
                newState.AddLiveCell(row, col);
            }

            UpdateState(newState);
        }

        public void ResetGame()
        {
            var newState = new GameState(_currentState.Rows, _currentState.Columns);
            UpdateState(newState);
        }

        public void RandomizeBoard()
        {
            var rand = new Random();
            var newState = new GameState(_currentState.Rows, _currentState.Columns);

            for (int row = 0; row < _currentState.Rows; row++)
            {
                for (int col = 0; col < _currentState.Columns; col++)
                {
                    if (rand.Next(100) < 20)  // 20% chance for a cell to be alive
                    {
                        newState.AddLiveCell(row, col);
                    }
                }
            }
            UpdateState(newState);
        }
    }
}
