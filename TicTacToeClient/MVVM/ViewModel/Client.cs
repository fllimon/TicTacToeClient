using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TicTacToe.GameLibrary.MVVM.Model;

namespace TicTacToeClient.MVVM.ViewModel
{
    class Client : ClientViewModel
    {
        private const string DEFAULT_FILE_LOG_PATH = "../../log.txt";
        private TcpClient _client = null;
        private string _port = "24000";
        private string _ip = "localhost";
        private string _name = "Player";
        private bool _keepRuning = true;
        private bool _brdVisibility = true;
        private ObservableCollection<Marker> _gameField = null;
        private Player _player = null;
        private char[] _buffer = null;

        public Client()
        {
            _client = new TcpClient();
            _player = new Player();
            _buffer = new char[1024];

            Press = new Command(o =>
            {
                Marker current = o as Marker;
            });

            PressX = new Command(o =>
            {
                _player.PlayerType = PlayerData.X;
                _player.Name = Name;
                _player.PlayerStatus = PlayerStatus.Move;
                BorderVisibility = false;

                ConnectToServer();
            });

            PressO = new Command(o =>
            {
                _player.PlayerType = PlayerData.O;
                _player.Name = Name;
                _player.PlayerStatus = PlayerStatus.Await;
                BorderVisibility = false;

                ConnectToServer();
            });

            Exit = new Command(o =>
            {
                _client.Close();

                Environment.Exit(0);
            });
        }

        public ObservableCollection<Marker> GameField
        {
            get 
            {
                return _gameField; 
            }
            set
            {
                _gameField = value;

                OnPropertyChanged();
            }
        }

        public ICommand Press { get; set; }

        public ICommand PressO { get; set; }


        public ICommand PressX { get; set; }

        public ICommand Exit { get; set; }

        public string ServerIp 
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;

                OnPropertyChanged();
            }
        }

        public bool BorderVisibility
        {
            get
            {
                return _brdVisibility;
            }
            set
            {
                _brdVisibility = value;

                OnPropertyChanged();
            }
        }

        public string Port
        {
            get 
            {
                return _port; 
            }
            set 
            { 
                _port = value; 
                
                OnPropertyChanged(); 
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                OnPropertyChanged();
            }
        }

        public async Task ConnectToServer()
        {
            int port = int.Parse(Port);

            try
            {
                await _client.ConnectAsync(ServerIp, port);

                if (_client.Connected)
                {
                    SendDataToServer(_player);
                    Array.Clear(_buffer);

                    ReadGameFieldFromServer();
                }

            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        private async Task ReadGameFieldFromServer()
        {
            try
            {
                StreamReader reader = new StreamReader(_client.GetStream());
                
                while (_keepRuning)
                {
                    int count = await reader.ReadAsync(_buffer);

                    string data = new string(_buffer, 0, count);
                    
                    //List<Marker> mr = System.Text.Json.JsonSerializer.Deserialize<List<Marker>>(data);    // Exeption
                    
                    List<Marker> markers = JsonConvert.DeserializeObject<List<Marker>>(data);

                    GameField = new ObservableCollection<Marker>(markers);
                    
                    WriteLog(string.Format($"GameField: {GameField.Count}"));

                    if (GameField != null)
                    {
                        _keepRuning = false;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task SendDataToServer(object data)
        {
            if (data == null)
            {
                return;
            }

            await System.Text.Json.JsonSerializer.SerializeAsync(_client.GetStream(), data);
        }

        public void WriteLog(string data)
        {
            FileStream fs = new FileStream(DEFAULT_FILE_LOG_PATH, FileMode.OpenOrCreate);

            using (StreamWriter wr = new StreamWriter(fs))
            {
                wr.WriteLine(data);
            }
        }
    }
}
