using MahApps.Metro.Controls;
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
using TicTacToeClient.MVVM.View;
using TicTacToeClient.MVVM.ViewModel;

namespace TicTacToeClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Client _client = null;

        public MainWindow()
        {
            InitializeComponent();
            _client = new Client();
            DataContext = _client;
            _frame.DataContext = this;
        }

        private void StartServerClick(object sender, RoutedEventArgs e)
        {
            _frame.Content = new GameFieldPage();
            _startServer.Visibility = Visibility.Hidden;
        }

        private void _title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
