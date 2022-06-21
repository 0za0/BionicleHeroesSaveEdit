using BionicleHeroesSaveEditor.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BionicleHeroesSaveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ToaConfig> AllToaConfigs;
        public MainWindow()
        {
            InitializeComponent();
            AllToaConfigs = new List<ToaConfig>() { JallerConfig, HahliConfig, KonguConfig, MatoroConfig, HewkiiConfig, NuparuConfig };
            JallerConfig.ToaImage.Source = new BitmapImage(new Uri(@"/Images/ToaHeads/jaller.png", UriKind.Relative));
            KonguConfig.ToaImage.Source = new BitmapImage(new Uri(@"/Images/ToaHeads/kongu.png", UriKind.Relative));
            MatoroConfig.ToaImage.Source = new BitmapImage(new Uri(@"/Images/ToaHeads/matoro.png", UriKind.Relative));
            HewkiiConfig.ToaImage.Source = new BitmapImage(new Uri(@"/Images/ToaHeads/hewkii.png", UriKind.Relative));
            NuparuConfig.ToaImage.Source = new BitmapImage(new Uri(@"/Images/ToaHeads/nuparu.png", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Bionicle Save Games (.bin) | *.bin";
            ofd.Title = "Please Pick a SaveGame";
            ofd.InitialDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\EIDOS\BIONICLE Heroes\";
            ofd.ShowDialog();
            FilePath.Text = ofd.FileName;
            Helpers.FileOperations.ReadFile(ofd.FileName);
            var chkSum = Helpers.CheckSumFixer.FixCheckSum(Helpers.FileOperations.SaveDataBytes);
            CheckSumTextBox.Text = BitConverter.ToUInt32(chkSum).ToString();
            MoneyTextBox.Text = BitConverter.ToUInt32(Helpers.FileOperations.SaveDataBytes.GetRange(0xC4, 0xC7).ToArray()).ToString();
            MoneySpentTextBox.Text = BitConverter.ToUInt32(Helpers.FileOperations.SaveDataBytes.GetRange(0xC8, 0xCB).ToArray()).ToString();

            FillToaTextBoxes();
        }

        private void FillToaTextBoxes()
        {
            int healthStartLocation = 0x10;
            int upgradeStartLocation = 0x16;
            int weaponStartLocation = 0x1C;

            for (int i = 0; i < 6; i++)
            {
                AllToaConfigs[i].ToaHealthTextBox.Text = Helpers.FileOperations.SaveDataBytes[healthStartLocation].ToString();
                AllToaConfigs[i].ToaAbilityTextBox.Text = Helpers.FileOperations.SaveDataBytes[upgradeStartLocation].ToString();
                AllToaConfigs[i].ToaWeaponUpgradesTextBox.Text = Helpers.FileOperations.SaveDataBytes[weaponStartLocation].ToString();

                healthStartLocation++;
                upgradeStartLocation++;
                weaponStartLocation++;
            }

        }
        private void UpdateFile()
        {
            int healthStartLocation = 0x10;
            int upgradeStartLocation = 0x16;
            int weaponStartLocation = 0x1C;

            for (int i = 0; i < 6; i++)
            {
                Helpers.FileOperations.SaveDataBytes[healthStartLocation] = BitConverter.GetBytes(int.Parse(AllToaConfigs[i].ToaHealthTextBox.Text))[0];
                Helpers.FileOperations.SaveDataBytes[upgradeStartLocation] = BitConverter.GetBytes(int.Parse(AllToaConfigs[i].ToaAbilityTextBox.Text))[0];
                Helpers.FileOperations.SaveDataBytes[weaponStartLocation] = BitConverter.GetBytes(int.Parse(AllToaConfigs[i].ToaWeaponUpgradesTextBox.Text))[0];


                healthStartLocation++;
                upgradeStartLocation++;
                weaponStartLocation++;
            }
            int newMoney = int.Parse(MoneyTextBox.Text);
            byte[] newMoneyBytes = BitConverter.GetBytes(newMoney);
            Helpers.FileOperations.SaveDataBytes[0xC4] = newMoneyBytes[0];
            Helpers.FileOperations.SaveDataBytes[0xC5] = newMoneyBytes[1];
            Helpers.FileOperations.SaveDataBytes[0xC6] = newMoneyBytes[2];
            Helpers.FileOperations.SaveDataBytes[0xC7] = newMoneyBytes[3];

            int newMoneySpent = int.Parse(MoneySpentTextBox.Text);
            byte[] newMoneySpentBytes = BitConverter.GetBytes(newMoneySpent);
            Helpers.FileOperations.SaveDataBytes[0xC8] = newMoneySpentBytes[0];
            Helpers.FileOperations.SaveDataBytes[0xC9] = newMoneySpentBytes[1];
            Helpers.FileOperations.SaveDataBytes[0xCA] = newMoneySpentBytes[2];
            Helpers.FileOperations.SaveDataBytes[0xCB] = newMoneySpentBytes[3];

        }

        private void GenerateNew_Click(object sender, RoutedEventArgs e)
        {
            //This is very scuffed code because I just wrote this for the quick "Tech Demo" 
            //Didn't really do more so dont judge, thanks
            // int newMoney = int.Parse(MoneyAmount.Text);
            //byte[] newMoneyBytes = BitConverter.GetBytes(newMoney);

            //Helpers.FileOperations.SaveDataBytes[0xC4] = newMoneyBytes[0];
            //Helpers.FileOperations.SaveDataBytes[0xC5] = newMoneyBytes[1];
            //Helpers.FileOperations.SaveDataBytes[0xC6] = newMoneyBytes[2];
            //Helpers.FileReader.SaveDataBytes[0xC7] = newMoneyBytes[3];
            
            UpdateFile();
            var newChecksum = Helpers.CheckSumFixer.FixCheckSum(Helpers.FileOperations.SaveDataBytes);
            CheckSumTextBox.Text = BitConverter.ToUInt32(newChecksum).ToString();
            Helpers.FileOperations.SaveDataBytes[4096] = newChecksum[0];
            Helpers.FileOperations.SaveDataBytes[4097] = newChecksum[1];
            Helpers.FileOperations.SaveDataBytes[4098] = newChecksum[2];
            Helpers.FileOperations.SaveDataBytes[4099] = newChecksum[3];

            Helpers.FileOperations.WriteNewSaveFile("./savegame.bin");
        }
    }
}
