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
using TicTacToeClient.MVVM.ViewModel;

namespace TicTacToeClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for GameFieldPage.xaml
    /// </summary>
    public partial class GameFieldPage : Page
    {
        private Client _client = null;

        public GameFieldPage()
        {
            InitializeComponent();

            _client = new Client();
            DataContext = _client;
        }

        private void HideControls(object sender, RoutedEventArgs e)
        {
            _name.Visibility = Visibility.Hidden;
            _tbName.Visibility = Visibility.Hidden;
            _port.Visibility = Visibility.Hidden;
            _tbPort.Visibility = Visibility.Hidden;
            _ip.Visibility = Visibility.Hidden;
            _tbIp.Visibility = Visibility.Hidden;
            _connect.Visibility = Visibility.Hidden;
            _exit.Visibility = Visibility.Hidden;
            _playerX.Visibility = Visibility.Visible;
            _playerO.Visibility = Visibility.Visible;
        }
    }
}
