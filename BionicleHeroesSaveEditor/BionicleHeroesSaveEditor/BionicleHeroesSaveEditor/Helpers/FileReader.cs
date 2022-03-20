using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BionicleHeroesSaveEditor.Helpers
{
    internal class FileReader
    {
        public static List<byte> SaveDataBytes = new List<byte>();

        public static void ReadFile(string filepath)
        {
            //Error Handling when thing goes die
            SaveDataBytes = File.ReadAllBytes(filepath).ToList();
        }
        public static void WriteNewSaveFile(string path)
        {
            File.WriteAllBytes(path, SaveDataBytes.ToArray());
        }
    }
}
