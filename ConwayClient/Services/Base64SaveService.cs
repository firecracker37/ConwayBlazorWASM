using ConwayClient.Models;
using ConwayClient.Utils;
using System.Collections;

namespace ConwayClient.Services
{
    public class Base64SaveService : ISaveService
    {
        public GameBoard Load(string savedData, int rows, int cols)
        {
            // Decode the Base64 string to a byte array
            byte[] compressedBytes = Convert.FromBase64String(savedData);

            // Decompress the byte array
            byte[] decompressedBytes = ZipUtil.Decompress(compressedBytes);

            // Convert the byte array back to a BitArray
            BitArray bitArray = new BitArray(decompressedBytes);

            // Create a new GameBoard instance
            GameBoard gameBoard = new GameBoard(rows, cols); // Assuming default constructor sets the default size

            // Convert the BitArray back to your game board's state
            SetStateFromBitArray(gameBoard, bitArray);

            return gameBoard;
        }


        public string Save(GameBoard gameBoard)
        {
            // Convert the game board's state to a BitArray
            BitArray bitArray = ConvertStateToBitArray(gameBoard);

            // Convert the BitArray to a byte array
            byte[] byteArray = ZipUtil.BitArrayToByteArray(bitArray);

            // Compress the byte array
            byte[] compressedByteArray = ZipUtil.Compress(byteArray);

            // Convert the compressed byte array to a Base64 string for sharing
            return Convert.ToBase64String(compressedByteArray);
        }


        private BitArray ConvertStateToBitArray(GameBoard gameBoard)
        {
            BitArray bits = new BitArray(gameBoard.Rows * gameBoard.Columns);

            int index = 0;
            for (int row = 0; row < gameBoard.Rows; row++)
            {
                for (int col = 0; col < gameBoard.Columns; col++)
                {
                    bits[index++] = gameBoard.GetCellState(row, col);
                }
            }

            return bits;
        }

        private void SetStateFromBitArray(GameBoard gameBoard, BitArray bits)
        {
            if (bits.Length != gameBoard.Rows * gameBoard.Columns)
            {
                throw new ArgumentException("BitArray size doesn't match GameBoard size.");
            }

            int index = 0;
            for (int row = 0; row < gameBoard.Rows; row++)
            {
                for (int col = 0; col < gameBoard.Columns; col++)
                {
                    gameBoard.SetCellState(row, col, bits[index++]);
                }
            }
        }
    }
}
