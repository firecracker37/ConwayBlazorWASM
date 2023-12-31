﻿using ConwaysGameOfLife.Entities;

namespace ConwaysGameOfLife.Core
{
    public class GameState
    {
        private readonly HashSet<CellPosition> _liveCells = new HashSet<CellPosition>();
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }

        // Method to add a live cell to the state
        public void AddLiveCell(int row, int col)
        {
            _liveCells.Add(new CellPosition(row, col));
        }

        // Method to remove a live cell from the state
        public void RemoveLiveCell(int row, int col)
        {
            _liveCells.Remove(new CellPosition(row, col));
        }

        // Method to remove all live cells from the state
        public void RemoveAllCells()
        {
            _liveCells.Clear();
        }

        // Method to check if a position is alive in the current state
        public bool IsCellAlive(int row, int col)
        {
            return _liveCells.Contains(new CellPosition(row, col));
        }

        // Method to get all the live cells in the current state
        public IEnumerable<CellPosition> GetLiveCells()
        {
            return _liveCells;
        }

        public GameState Clone()
        {
            var clonedState = new GameState(this.Rows, this.Columns);
            foreach (var cell in this.GetLiveCells())
            {
                clonedState.AddLiveCell(cell.Row, cell.Col);
            }
            return clonedState;
        }
    }

}
