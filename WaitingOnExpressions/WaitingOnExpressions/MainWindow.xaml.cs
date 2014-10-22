using System.Windows;
using WaitingOnExpressions.Logic;
using System.Linq;

namespace WaitingOnExpressions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Model Model { get; set; }

        public MainWindow()
        {
            Model = new Model();
            DataContext = Model;

            InitializeComponent();

            PrepareForDeactivation();


        }

        public async void PrepareForDeactivation()
        {
            while (true)
            {
                Model.Treshold = 1;
                Model.Keyword = string.Empty;

                //await Until.BecomesTrue(() =>
                //    Model.Keyword == Title
                //    );

                var criticalWindow = new CriticalErrorWindow { DataContext = Model };
                criticalWindow.ShowDialog();

                await Until.BecomesTrue(() => criticalWindow.IsLoaded == false);

                /* Model.Keyword != string.Empty &&
                    Model.Logs.Count(x => x.Message.Contains(Model.Keyword))
                        > Model.Treshold * Model.Logs.Count */

                MessageBox.Show("Aha!!!");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Title = "Bla";
        }
    }
}

