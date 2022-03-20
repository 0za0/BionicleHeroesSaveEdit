using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BionicleHeroesSaveEditor.Helpers
{
    internal static class CheckSumFixer
    {
        public static byte[] FixCheckSum(List<byte> saveFile)
        {
            uint checkSum = 0;
            for (int i = 0; i < (4096 - 0) / 4; i++)
            {
                byte[] readBytes = saveFile.Skip(4 * i).Take(4).ToArray();
                checkSum += BitConverter.ToUInt32(readBytes);
            }
            uint oldCheckSum = BitConverter.ToUInt32(saveFile.GetRange(saveFile.Count - 4, 4).ToArray());
            Debug.WriteLine($"Old Checksum = {oldCheckSum}");
            Debug.WriteLine($"New Checksum = {checkSum}");
            return BitConverter.GetBytes(checkSum);
        }
    }
}
