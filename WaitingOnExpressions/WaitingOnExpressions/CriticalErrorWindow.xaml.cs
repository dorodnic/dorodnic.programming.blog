using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;
using WaitingOnExpressions.Logic;

namespace WaitingOnExpressions
{
    /// <summary>
    /// Interaction logic for CriticalErrorWindow.xaml
    /// </summary>
    public partial class CriticalErrorWindow : Window
    {
        public CriticalErrorWindow()
        {
            InitializeComponent();
        }

        private async void SuccessWorkflow()
        {
            while (true)
            {
                btnUnlock.IsEnabled = false;
                txtBox.IsEnabled = true;

                await Until.BecomesTrue(() => txtBox.Text == Title);

                btnUnlock.IsEnabled = true;
                txtBox.IsEnabled = false;

                await Until.IsClicked(btnUnlock);

                MessageBox.Show("Well done!");
            }
        }

        private async void CriticalErrorWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var random = new Random();

            SuccessWorkflow();

            while (IsVisible)
            {
                Title = string.Join(string.Empty,
                    Enumerable
                    .Range(1, 5)
                    .Select(x => random.Next() % 10)
                    .Select(x => x.ToString(CultureInfo.InvariantCulture)));

                await Task.Delay(5000);
            }
        }
    }
}
