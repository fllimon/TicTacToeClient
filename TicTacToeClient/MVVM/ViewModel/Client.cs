using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private string _ip = "127.0.0.1";
        private string _name = "Player";
        private bool _brdVisibility = true;
        private ObservableCollection<Marker> _gameField = null;
        private Player _player = null;
        private byte[] _bufferRecive = null;
        
        public Client()
        {
            _client = new TcpClient();
            _player = new Player();

            Press = new Command(o =>
            {
                if (_player.PlayerStatus == PlayerStatus.Await)
                {
                    return;
                }

                Marker current = (Marker)o;

                if (current != null)
                {
                    SendPlayerDataToServer(current);
                }
            });

            PressX = new Command(o =>
            {
                _player.PlayerType = PlayerData.X;
                _player.Name = Name;
                _player.PlayerStatus = PlayerStatus.Move;
                BorderVisibility = false;

                ConnectToServer();
                SendPlayerDataToServer(_player);

            });

            PressO = new Command(o =>
            {
                _player.PlayerType = PlayerData.O;
                _player.Name = Name;
                _player.PlayerStatus = PlayerStatus.Await;
                BorderVisibility = false;

                ConnectToServer();
                SendPlayerDataToServer(_player);
            });

            Exit = new Command(o =>
            {
                _client.Close();

                Environment.Exit(0);
            });
        }

        #region ================ PROPERTIES =====================

        public Player Player 
        {
            get 
            { 
                return _player; 
            }
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

        #endregion

        public void ConnectToServer()
        {
            IPAddress ip = IPAddress.Parse(ServerIp);
            int port = int.Parse(Port);

            try
            {
                _client.BeginConnect(ip, port, OnClientCompliteConnected, _client);
            }
            catch (SocketException ex)
            {

                throw;
            }
        }

        private void OnClientCompliteConnected(IAsyncResult ar)
        {
            TcpClient tcpClient = null;

            try
            {
                tcpClient = ar.AsyncState as TcpClient;
                tcpClient.EndConnect(ar);

                _bufferRecive = new byte[1024];
                _client.GetStream().BeginRead(_bufferRecive, 0, _bufferRecive.Length, OnCompleteReadData, _client);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);

                _client.Close();
            }
        }


        private void OnCompleteReadData(IAsyncResult ar)
        {
            TcpClient client = null;

            try
            {
                client = ar.AsyncState as TcpClient;
                int byteCount = client.GetStream().EndRead(ar);

                if (byteCount == 0)
                {
                    return;
                }

                string data = Encoding.UTF8.GetString(_bufferRecive, 0, byteCount);

                if (data.Contains("PlayerStatus"))
                {
                    Player player = JsonSerializer.Deserialize<Player>(data);
                    _player.PlayerStatus = player.PlayerStatus;
                }
                else
                {
                    List<Marker> mrkr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Marker>>(data);
                    GameField = new ObservableCollection<Marker>(mrkr);
                }

                _bufferRecive = new byte[1024];
                client.GetStream().BeginRead(_bufferRecive,0, _bufferRecive.Length,OnCompleteReadData, client);
            }
            catch (Exception ex)
            {
                _client.Close();
            }
        }

        private void SendPlayerDataToServer(object data)
        {
            if (_client == null)
            {
                return;
            }

            if (data == null)
            {
                return;
            }

            string serealizeData = JsonSerializer.Serialize(data);

            byte[] bufferSend = new byte[1024];
            bufferSend = Encoding.UTF8.GetBytes(serealizeData);

            try
            {
                if (_client.Client.Connected)
                {
                    _client.GetStream().BeginWrite(bufferSend, 0, bufferSend.Length, OnCompleteWriteDataToServer, _client);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void OnCompleteWriteDataToServer(IAsyncResult ar)
        {
            TcpClient client = null;

            try
            {
                client = ar.AsyncState as TcpClient;
                client.GetStream().EndWrite(ar);
            }
            catch (Exception ex)
            {

                throw;
            }
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
