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
using WpfHexaEditor.Core.EventArguments;

namespace HeroSaveEditDebugging
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _internalChange = false;
        public MainWindow()
        {
            InitializeComponent();
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

        }

        private void FileDiffBytesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BlockItemProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
