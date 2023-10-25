using ConwayClient.Models;

namespace ConwayClient.Services
{
    public interface ISaveService
    {
        string Save(GameBoard gameBoard);
        GameBoard Load(string savedData, int rows, int cols);
    }
}
