using ConwaysGameOfLife.Core;
using ConwaysGameOfLife.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.History
{
    public class HistoryManager
    {
        private readonly List<HashSet<CellPosition>> _history = new List<HashSet<CellPosition>>();
        private const int MAX_HISTORY_SIZE = 100;  // Adjust as needed

        // Adds a game state to the history
        public void AddToHistory(GameState state)
        {
            if (_history.Count >= MAX_HISTORY_SIZE)
            {
                _history.RemoveAt(0);
            }

            _history.Add(new HashSet<CellPosition>(state.GetLiveCells()));
        }

        // Retrieves a game state from the history by its index
        public GameState GetStateFromHistory(int index, int rows, int columns)
        {
            if (index < 0 || index >= _history.Count)
            {
                return null;  // Or throw an exception
            }

            var liveCells = _history[index];
            var retrievedState = new GameState(rows, columns);

            foreach (var cell in liveCells)
            {
                retrievedState.AddLiveCell(cell.Row, cell.Col);
            }

            return retrievedState;
        }

        // Removes a game state from the history by its index
        public void RemoveFromHistory(int index)
        {
            if (index < 0 || index >= _history.Count)
            {
                return;  // Or throw an exception
            }

            _history.RemoveAt(index);
        }

        // Clears the entire history
        public void ClearHistory()
        {
            _history.Clear();
        }

        public GameState UndoLastState(int rows, int columns)
        {
            if (IsHistoryEmpty)
            {
                return null;  // Or throw an exception
            }

            var lastStateCells = _history[_history.Count - 1];
            _history.RemoveAt(_history.Count - 1);

            var retrievedState = new GameState(rows, columns);
            foreach (var cell in lastStateCells)
            {
                retrievedState.AddLiveCell(cell.Row, cell.Col);
            }

            return retrievedState;
        }

        // Peeks at the last game state added to the history without removing it
        public GameState PeekLastState(int rows, int columns)
        {
            if (IsHistoryEmpty)
            {
                return null;  // Or throw an exception
            }

            var lastStateCells = _history[_history.Count - 1];

            var retrievedState = new GameState(rows, columns);
            foreach (var cell in lastStateCells)
            {
                retrievedState.AddLiveCell(cell.Row, cell.Col);
            }

            return retrievedState;
        }

        // Returns the number of saved states
        public int HistoryCount => _history.Count;

        // Checks if the history is empty
        public bool IsHistoryEmpty => _history.Count == 0;
    }
}
