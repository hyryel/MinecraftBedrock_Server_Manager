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

namespace Minecraft_Bedrock_server_Wrapper
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

        private ServerManager _server;

        private void MnuQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
        private void MnuStopServer_Click(object sender, RoutedEventArgs e)
        {
            _server.SendCommand("stop");
        }

        System.Collections.ObjectModel.ObservableCollection<string> _infos = new System.Collections.ObjectModel.ObservableCollection<string>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstInfo.DataContext = _infos;
            _server = new ServerManager("G:\\jeux\\Minecraft bedrock serveur\\bedrock_server.exe");
            _server.StateUpdated += _server_StateUpdated;
        }

        private void _server_StateUpdated(object sender,ServerUpdatedEventArgs e)
        {
            _infos.Add(e.Message);
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            _server.Start();
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            _server.Stop();
        }
    }
}
