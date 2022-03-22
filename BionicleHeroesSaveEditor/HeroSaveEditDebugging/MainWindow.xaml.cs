using HeroSaveEditDebugging.Helper;
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
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.EventArguments;
using WpfHexaEditor.Core.MethodExtention;
using WpfHexEditor.Sample.BinaryFilesDifference;

namespace HeroSaveEditDebugging
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _internalChange = false;
        List<ByteDifference> _differences = null;
        List<BlockListItem> _blockListItem = new List<BlockListItem>();
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            // FirstFile.FileName = @"C:\Users\nullpo\AppData\Local\EIDOS\BIONICLE Heroes\savegame_0\savegame.bin";
            // SecondFile.FileName = @"C:\Users\nullpo\AppData\Local\EIDOS\BIONICLE Heroes\savegame_0\savegame.bin";
            FileHelper.CreateSnapshotDirectory();//Init
            FillComboBoxes();
        }
        private void FillComboBoxes()
        {
            FirstFileComboBox.Items.Clear();
            SecondFileComboBox.Items.Clear();

            foreach (var item in FileHelper.GetSnapshotNames())
            {
                FirstFileComboBox.Items.Add(item);
                SecondFileComboBox.Items.Add(item);
            }
        }
        #region Synchronise the two hexeditor
        private void FirstFile_VerticalScrollBarChanged(object sender, ByteEventArgs e)
        {
            if (_internalChange) return;

            _internalChange = true;
            SecondFile.SetPosition(e.BytePositionInStream);
            _internalChange = false;
        }

        private void SecondFile_VerticalScrollBarChanged(object sender, ByteEventArgs e)
        {
            if (_internalChange) return;

            _internalChange = true;
            FirstFile.SetPosition(e.BytePositionInStream);
            _internalChange = false;
        }
        #endregion

        private void FindDifferenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (FirstFile.FileName == string.Empty || SecondFile.FileName == string.Empty)
            {
                MessageBox.Show("LOAD TWO FILE!!", "HexEditor sample", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Application.Current.MainWindow.Cursor = Cursors.Wait;
            FindDifference();
            Application.Current.MainWindow.Cursor = null;
        }
        private void ClearUI()
        {
            FileDiffBytesList.Items.Clear();
            FirstFile.ClearCustomBackgroundBlock();
            SecondFile.ClearCustomBackgroundBlock();
            SecondFile.ClearAllChange();
            _blockListItem.Clear();
            _differences = null;
        }

        private void FileDiffBytesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BlockItemProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void CreateSnapshot_Clicked(object sender, RoutedEventArgs e)
        {
            FileHelper.CreateSnapShot(SnapshotName.Text);
            FillComboBoxes();
        }

        private void FirstFileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FirstFile.FileName = @$"{FileHelper.SnapShotPath}/{FirstFileComboBox.SelectedValue}/savegame.bin";

        }

        private void SecondFileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SecondFile.FileName = @$"{FileHelper.SnapShotPath}/{SecondFileComboBox.SelectedValue}/savegame.bin";

        }
        private void FindDifference()
        {
            ClearUI();

            if (FirstFile.FileName == string.Empty || SecondFile.FileName == string.Empty) return;

            FileDiffBlockList.Children.Clear();

            //load the difference
            _differences = FirstFile.Compare(SecondFile).ToList();

            //Load list of difference
            var cbb = new CustomBackgroundBlock();
            int j = 0;

            foreach (ByteDifference byteDifference in _differences)
            {
                //create or update custom background block
                if (j == 0)
                    cbb = new CustomBackgroundBlock(byteDifference.BytePositionInStream, ++j, RandomBrushes.PickBrush());
                else
                    cbb.Length = ++j;

                if (!_differences.Any(c => c.BytePositionInStream == byteDifference.BytePositionInStream + 1))
                {
                    j = 0;

                    new BlockListItem(cbb).With(c =>
                    {

                        c.Click += BlockItem_Click;
                        _blockListItem.Add(c);
                    });

                    //add to hexeditor
                    FirstFile.CustomBackgroundBlockItems.Add(cbb);
                    SecondFile.CustomBackgroundBlockItems.Add(cbb);
                }
            }

            //Update progressbar
            BlockItemProgress.Maximum = _blockListItem.Count();
            UpdateListofBlockItem();

            //refresh editor
            FirstFile.RefreshView();
            SecondFile.RefreshView();


        }

        private void BlockItem_Click(object? sender, EventArgs e)
        {
            if (_internalChange) return;
            if (sender is not BlockListItem blockitm) return;
            if (_differences is null) return;

            //Clear UI
            FileDiffBytesList.Items.Clear();

            _internalChange = true;
            FirstFile.SetPosition(blockitm.CustomBlock.StartOffset, 1);
            SecondFile.SetPosition(blockitm.CustomBlock.StartOffset, 1);
            _internalChange = false;

            //Load list of byte difference
            foreach (ByteDifference byteDifference in _differences
                .Where(c => c.BytePositionInStream >= blockitm.CustomBlock.StartOffset &&
                            c.BytePositionInStream <= blockitm.CustomBlock.StopOffset))
            {
                byteDifference.Color = blockitm.CustomBlock.Color;
                FileDiffBytesList.Items.Add(new ByteDifferenceListItem(byteDifference));
            }
        }

        private void UpdateListofBlockItem()
        {
            FileDiffBlockList.Children.Clear();

            var nbViewItem = (int)BlockItemProgress.Value + (int)(FileDiffBlockList.ActualHeight / new BlockListItem().Height);

            for (int i = (int)BlockItemProgress.Value; i < nbViewItem; i++)
            {
                if (i < _blockListItem.Count)
                    FileDiffBlockList.Children.Add(_blockListItem[i]);
            }
        }

    }
}

