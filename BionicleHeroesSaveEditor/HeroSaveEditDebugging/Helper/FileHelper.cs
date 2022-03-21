using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace HeroSaveEditDebugging.Helper
{
    internal class FileHelper
    {
        private static string SaveDataPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\EIDOS\BIONICLE Heroes\";
        public static string SnapShotPath = Path.Combine(SaveDataPath, "Snapshots");
        private static string SaveGameSlot = "savegame_2";
        private static int SnapShotAmount = 0;
        public static void CreateSnapshotDirectory()
        {
            if (!Directory.Exists(SnapShotPath))
                Directory.CreateDirectory(SnapShotPath);
            GetSnapShotsCount();
        }
        public static void CreateSnapShot(string snapshotName)
        {
            string snapShotFolder = Path.Join(SnapShotPath, $"{snapshotName}");
            foreach (var file in Directory.GetFiles(Path.Join(SaveDataPath, SaveGameSlot)))
            {
                // Create Dir if it dont exist 
                if (!Directory.Exists(Path.Join(SnapShotPath, $"{snapshotName}")))
                {
                    Directory.CreateDirectory(Path.Join(SnapShotPath, $"{snapshotName}"));
                    File.Copy(file, snapShotFolder + $@"/{Path.GetFileName(file)}", true);
                } //Otherwise just copy to directoy
                else
                    File.Copy(file, snapShotFolder + $@"/{Path.GetFileName(file)}", true);

            }
            SnapShotAmount++;
        }
        private static int GetSnapShotsCount() =>
             Directory.GetFiles(Path.Combine(SaveDataPath, "Snapshots")).Count();
        public static List<string> GetSnapshotNames()
        {
            List<string> snapshotnames = new List<string>();
            foreach (var item in Directory.GetDirectories(SnapShotPath))
            {
                snapshotnames.Add(Path.GetFileName(item));
            }
            return snapshotnames;
        }
    }
}
