namespace ConwayClient.Models
{
    public class Cell
    {
        public int X { get; set; }  // X coordinate
        public int Y { get; set; }  // Y coordinate
        public bool IsAlive { get; set; }  // Current state of the cell

        public Cell(int x, int y, bool isAlive)
        {
            X = x;
            Y = y;
            IsAlive = isAlive;
        }
    }

}
