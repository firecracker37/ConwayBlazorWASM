using ConwaysGameOfLife.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
