namespace ConwayClient.Models
{
    public class Cell
    {
        public int Row { get; }
        public int Column { get; }
        public bool IsAlive { get; }

        public Cell(int row, int column, bool isAlive)
        {
            Row = row;
            Column = column;
            IsAlive = isAlive;
        }
    }
}
