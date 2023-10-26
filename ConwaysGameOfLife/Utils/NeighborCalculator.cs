using ConwaysGameOfLife.Core;
using ConwaysGameOfLife.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.Utils
{
    public static class NeighborCalculator
    {
        // Returns the positions of the neighbors for a given cell position
        public static IEnumerable<CellPosition> GetNeighbors(CellPosition cell, GameState state)
        {
            // Relative positions of the 8 neighbors
            int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newRow = cell.Row + rowOffsets[i];
                int newCol = cell.Col + colOffsets[i];

                if (newRow >= 0 && newRow < state.Rows && newCol >= 0 && newCol < state.Columns)
                {
                    yield return new CellPosition(newRow, newCol);
                }
            }
        }

        // Returns the live neighbors of a given cell
        public static IEnumerable<CellPosition> GetLiveNeighbors(CellPosition cell, GameState state)
        {
            foreach (var neighbor in GetNeighbors(cell, state))
            {
                if (state.IsCellAlive(neighbor.Row, neighbor.Col))
                {
                    yield return neighbor;
                }
            }
        }

        // Returns the count of live neighbors for a given cell
        public static int CountLiveNeighbors(CellPosition cell, GameState state)
        {
            return GetLiveNeighbors(cell, state).Count();
        }
    }
}
