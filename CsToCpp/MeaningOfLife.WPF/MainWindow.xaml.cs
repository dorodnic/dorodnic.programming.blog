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

namespace MeaningOfLife.WPF
{
    using Cpp.CLI;
    using Microsoft.Win32;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Native Library|MeaningOfLife.Cpp.dll",
                InitialDirectory = Environment.CurrentDirectory
            };

            var result = fileOpenDialog.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                Logic.InitializeLibrary(fileOpenDialog.FileName);

                using (var wrapper = new Logic())
                {
                    MessageBox.Show("The answer is " + wrapper.Get());
                }
            }
        }
    }
}
