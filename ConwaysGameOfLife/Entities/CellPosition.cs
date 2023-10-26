using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.Entities
{
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
