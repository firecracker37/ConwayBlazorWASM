using K4os.Compression.LZ4;
using System.Collections;

namespace ConwayClient.Utils
{
    public class ZipUtil
    {
        public static byte[] Compress(byte[] input)
        {
            var maxSize = LZ4Codec.MaximumOutputSize(input.Length);
            var output = new byte[maxSize + sizeof(int)];  // +4 bytes to store the decompressed size

            var actualSize = LZ4Codec.Encode(input, 0, input.Length, output, sizeof(int), maxSize);

            // Store the decompressed size at the beginning of the compressed data
            BitConverter.GetBytes(input.Length).CopyTo(output, 0);

            // Resize the result array to the actual compressed size + 4 bytes for the decompressed size
            Array.Resize(ref output, actualSize + sizeof(int));
            return output;
        }

        public static byte[] Decompress(byte[] input)
        {
            // Retrieve the decompressed size from the first 4 bytes of the input
            var decompressedSize = BitConverter.ToInt32(input, 0);

            var output = new byte[decompressedSize];
            LZ4Codec.Decode(input, sizeof(int), input.Length - sizeof(int), output, 0, output.Length);
            return output;
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            int numBytes = bits.Count / 8;
            if (bits.Count % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            bits.CopyTo(bytes, 0);

            return bytes;
        }
    }
}
