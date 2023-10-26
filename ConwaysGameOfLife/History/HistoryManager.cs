using ConwaysGameOfLife.Core;
using ConwaysGameOfLife.Entities;

namespace ConwaysGameOfLife.History
{
    public class HistoryManager
    {
        private readonly List<HistoryEntry> _history = new List<HistoryEntry>();
        private const int MAX_HISTORY_SIZE = 100;  // Adjust as needed

        // Adds a game state to the history
        public void AddToHistory(GameState state)
        {
            if (_history.Count >= MAX_HISTORY_SIZE)
            {
                _history.RemoveAt(0);
            }

            var historyEntry = new HistoryEntry(state.Rows, state.Columns, new HashSet<CellPosition>(state.GetLiveCells()));
            _history.Add(historyEntry);
            Console.WriteLine("Adding state to history");
        }

        // Retrieves a game state from the history by its index
        public GameState GetStateFromHistory(int index)
        {
            if (index < 0 || index >= _history.Count)
            {
                return null;  // Or throw an exception
            }

            var historyEntry = _history[index];
            var retrievedState = new GameState(historyEntry.Rows, historyEntry.Columns);

            foreach (var cell in historyEntry.LiveCells)
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

        public GameState UndoLastState()
        {
            if (IsHistoryEmpty)
            {
                return null;  // Or throw an exception
            }
            Console.WriteLine("UndoLastState() in History Manager has fired");
            var lastHistoryEntry = _history[_history.Count - 1];
            _history.RemoveAt(_history.Count - 1);

            var retrievedState = new GameState(lastHistoryEntry.Rows, lastHistoryEntry.Columns);
            foreach (var cell in lastHistoryEntry.LiveCells)
            {
                retrievedState.AddLiveCell(cell.Row, cell.Col);
            }

            return retrievedState;
        }

        // Peeks at the last game state added to the history without removing it
        public GameState PeekLastState()
        {
            if (IsHistoryEmpty)
            {
                return null;  // Or throw an exception
            }

            var lastHistoryEntry = _history[_history.Count - 1];

            var retrievedState = new GameState(lastHistoryEntry.Rows, lastHistoryEntry.Columns);
            foreach (var cell in lastHistoryEntry.LiveCells)
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
