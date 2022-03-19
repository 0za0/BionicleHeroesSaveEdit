using System;
using System.Collections.Generic;

namespace CheckSumFixer
{
    class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "savegame.bin";
            List<byte> SaveData = File.ReadAllBytes(filePath).ToList();


            byte[] correctChecksum = CheckSumFixer.FixCheckSum(SaveData, 0, 4096, 0);
            uint x = BitConverter.ToUInt32(SaveData.GetRange(0xC4, 0xC7).ToArray());
            Console.WriteLine($"Money: {x}");
        }
    }
}