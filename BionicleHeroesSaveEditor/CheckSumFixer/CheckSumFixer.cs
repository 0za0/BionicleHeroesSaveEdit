using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckSumFixer
{
    // Thank you very very very very much Github User PollyPlayz
    public static class CheckSumFixer
    {
        public static byte[] FixCheckSum(List<byte> saveFile, uint startIndex, uint endIndex, uint salt)
        {
            uint checkSum = salt;
            for (int i = 0; i < (endIndex - startIndex) / 4; i++)
            {
                byte[] readBytes = saveFile.Skip(4 * i).Take(4).ToArray();
                checkSum += BitConverter.ToUInt32(readBytes);
            }
            uint oldCheckSum = BitConverter.ToUInt32(saveFile.GetRange(saveFile.Count - 4, 4).ToArray());
            Console.WriteLine($"Old Checksum = {oldCheckSum}");
            Console.WriteLine($"New Checksum = {checkSum}");
            return BitConverter.GetBytes(checkSum);
        }
    }
}
