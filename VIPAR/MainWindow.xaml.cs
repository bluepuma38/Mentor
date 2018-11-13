using System;
using System.Windows;
using System.Windows.Navigation;

namespace VIPAR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static NavigationService Navigation;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartClicked(object sender, RoutedEventArgs e)
        {
            switch((string)ReportType.SelectionBoxItem)
            {
                case "Kick Off":
                    {
                        KickOffReport win = new KickOffReport();
                        var host = new Window();
                        host.Height = 690;
                        host.Width = 810;
                        host.Content = win;
                        host.Show();
                        break;
                    }
                default:
                    return;
            }
            ReportType.SelectedIndex = -1;
        }
    }
}
