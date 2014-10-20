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
using UMDH.Parser;

namespace UMDH.Visualizer
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

        public Codebase Codebase { 
            get 
            { 
                return DataContext as Codebase; 
            }
            set
            {
                DataContext = value;
            }
        }

        public List<string> mVisualStudioOptions = new List<string> { "2005", "2008", "2010", "2012" };
        public List<string> VisualStudioOptions { get { return mVisualStudioOptions; }}
        private string mVisualStudioVersion = "2010";
        public string VisualStudioVersion { 
            get { return mVisualStudioVersion; } 
            set { mVisualStudioVersion = value; } 
        }


        public TabItem SelectedTab
        {
            get { return (TabItem)GetValue(SelectedTabProperty); }
            set { SetValue(SelectedTabProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTab.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTabProperty =
            DependencyProperty.Register("SelectedTab", typeof(TabItem), typeof(MainWindow), new PropertyMetadata(null));



        public Backtrace SelectedLeak
        {
            get { return (Backtrace)GetValue(SelectedLeakProperty); }
            set { SetValue(SelectedLeakProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedLeak.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedLeakProperty =
            DependencyProperty.Register("SelectedLeak", typeof(Backtrace), typeof(MainWindow),
            new FrameworkPropertyMetadata(null, OnSelectedLeakChange));

        public static void OnSelectedLeakChange(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var _this = obj as MainWindow;

            _this.SelectedTab = _this.tabLeakView;
        }



        public LineOfCode SelectedLine
        {
            get { return (LineOfCode)GetValue(SelectedLineProperty); }
            set { SetValue(SelectedLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedLineProperty =
            DependencyProperty.Register("SelectedLine", typeof(LineOfCode), typeof(MainWindow),
            new FrameworkPropertyMetadata(null, OnSelectedLineChange));

        public static void OnSelectedLineChange(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var _this = obj as MainWindow;

            _this.SelectedTab = _this.tabLineView;
        }



        public Module SelectedModule
        {
            get { return (Module)GetValue(SelectedModuleProperty); }
            set { SetValue(SelectedModuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedModule.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedModuleProperty =
            DependencyProperty.Register("SelectedModule", typeof(Module), typeof(MainWindow), new FrameworkPropertyMetadata(null, OnSelectedModuleChange));

        public static void OnSelectedModuleChange(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var _this = obj as MainWindow;

            _this.SelectedTab = _this.tabModuleView;
        }


        private void Load(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                var dlg = new OpenFileDialog
                {
                    Filter = "All Files|*.*",
                    CheckFileExists = true,
                    Multiselect = false,
                    Title = "Please choose UMDH delta file"
                };
                if (dlg.ShowDialog().Value)
                {
                    path = dlg.FileName;
                }
                else return;
            }

            try
            {
                var loadingWindow = new ProgressWindow();
                loadingWindow.Title = "Loading - " + path;
                
                bool continueWithLoading = true;
                loadingWindow.Closed += (s, e) =>
                    {
                        continueWithLoading = false;
                    };

                Codebase codebase = null;
                Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            codebase = UMDHParser.Parse(path, (progress) =>
                                {
                                    Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            loadingWindow.progressBar.Value = progress;
                                        }));
                                });
                            codebase.Normalize();

                            Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    if (continueWithLoading)
                                    {
                                        codebase.Owner = this;
                                        Codebase = codebase;
                                        Title = "UMDH Visualize - " + path;
                                        SelectedTab = tabRawView;
                                    }

                                    loadingWindow.Close();
                                }));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Cannot parse file \"{0}\":\r\n{1}", path, ex));
                        }
                    });

                loadingWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot parse file \"{0}\":\r\n{1}", path, ex));
            }
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            VisualStudioVersion = VisualStudioHelper.DetectVersion(VisualStudioOptions);

            if (Environment.GetCommandLineArgs().Count() == 2)
            {
                Load(Environment.GetCommandLineArgs()[1]);
            }
            else
            {
                Load();
            }
        }

        private void GotoRawView(object sender, RoutedEventArgs e)
        {
            SelectedTab = tabRawView;
            txtRaw.ScrollToLine(SelectedLeak.StartLine);
            txtRaw.ScrollToLine(SelectedLeak.EndLine);
        }

        private void GotoSelectedLeak(object sender, MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var dc = fe.DataContext as Backtrace;
            SelectedLeak = null;
            SelectedLeak = dc;
        }

        private void GotoSelectedModule(object sender, MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var dc = fe.DataContext as Module;
            SelectedModule = null;
            SelectedModule = dc;
        }

        private void GotoSelectedLine(object sender, MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var dc = fe.DataContext as LineOfCode;
            SelectedLine = null;
            SelectedLine = dc;
        }

        private void GotoSelectedModuleFromLine(object sender, MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var dc = fe.DataContext as LineOfCode;
            SelectedModule = dc.Module;

            e.Handled = true;
        }

        private void OpenDocument(object sender, MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var dc = fe.DataContext as LineOfCode;

            try
            {
                var vsObjectName = VisualStudioHelper.GetVisualStudioObjectName(VisualStudioVersion);
                VisualStudioHelper.OpenInVisualStudio(vsObjectName, dc.SourceFile.FullPath, dc.LineNumber);
            }
            catch
            {
                foreach (var version in VisualStudioOptions)
                {
                    try
                    {
                        var vsObjectName = VisualStudioHelper.GetVisualStudioObjectName(version);
                        VisualStudioHelper.OpenInVisualStudio(vsObjectName, dc.SourceFile.FullPath, dc.LineNumber);
                        break;
                    }
                    catch
                    {

                    }
                }

                try
                {
                    var vsObjectName = VisualStudioHelper.GetVisualStudioObjectName();
                    VisualStudioHelper.OpenInVisualStudio(vsObjectName, dc.SourceFile.FullPath, dc.LineNumber);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Cannot open line of code in Visual Studio!\r\n" + ex.ToString());
                }
            }

            e.Handled = true;
        }

        private void GotoModuleFromSelectedLine(object sender, RoutedEventArgs e)
        {
            SelectedModule = null;
            SelectedModule = SelectedLine.Module;
        }
        
        
    }
}
