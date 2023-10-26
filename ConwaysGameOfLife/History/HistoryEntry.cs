using ConwaysGameOfLife.Entities;

namespace ConwaysGameOfLife.History
{
    public class HistoryEntry
    {
        public int Rows { get; }
        public int Columns { get; }
        public HashSet<CellPosition> LiveCells { get; }

        public HistoryEntry(int rows, int columns, HashSet<CellPosition> liveCells)
        {
            Rows = rows;
            Columns = columns;
            LiveCells = liveCells;
        }
    }
}
