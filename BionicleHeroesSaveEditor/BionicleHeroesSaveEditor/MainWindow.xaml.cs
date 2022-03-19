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
        public MainWindow()
        {
            InitializeComponent();
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
            MoneyAmount.Text = BitConverter.ToUInt32(Helpers.FileOperations.SaveDataBytes.GetRange(0xC4, 0xC7).ToArray()).ToString();
        }

        private void GenerateNew_Click(object sender, RoutedEventArgs e)
        {
            //This is very scuffed code because I just wrote this for the quick "Tech Demo" 
            //Didn't really do more so dont judge, thanks
            int newMoney = int.Parse(MoneyAmount.Text);
            byte[] newMoneyBytes = BitConverter.GetBytes(newMoney);

            Helpers.FileOperations.SaveDataBytes[0xC4] = newMoneyBytes[0];
            Helpers.FileOperations.SaveDataBytes[0xC5] = newMoneyBytes[1];
            Helpers.FileOperations.SaveDataBytes[0xC6] = newMoneyBytes[2];
            //Helpers.FileReader.SaveDataBytes[0xC7] = newMoneyBytes[3];

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
