using Assets.Scripts.GameState;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviours
{
    public class Client : MonoBehaviour
    {
        private bool _socketReady;
        private TcpClient _socket;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public bool IsHost { get; set; }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            GameSetup.Instance.Client = this;
        }

        //public bool ConnectToServer(string host = "192.168.1.88", int port = 8088)
        public bool ConnectToServer(string host = "127.0.0.1", int port = 8088)
        {
            // If already connected, ignore
            if (_socketReady) return false;

            try
            {
                _socket = new TcpClient(host, port);
                _stream = _socket.GetStream();
                _writer = new StreamWriter(_stream);
                _reader = new StreamReader(_stream);
                _socketReady = true;

                ReceiveAsync(_socket);
            }
            catch (Exception e)
            {
                Debug.Log("Socket error : " + e.Message);
            }

            return _socketReady;
        }

        private async Task ReceiveAsync(TcpClient client)
        {
            try
            {
                // Begin receiving the data from the remote device. 
                var reader = new StreamReader(client.GetStream());
                var line = await reader.ReadLineAsync();
                Debug.Log("Received line : " + line);

                var mw = JsonConvert.DeserializeObject<MessageWrapper>(line);
                OnIncomingData(mw);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private async void Update()
        {
            if (_socketReady)
            {
                //await ReceiveAsync(_socket);
            }
        }

        private void OnIncomingData(MessageWrapper messageWrapper)
        {

            if (messageWrapper.Type == MessageType.PlayerGuid)
            {
                Debug.Log("(player guid) : " + messageWrapper.Message.ToString());

                var guid = Guid.Parse(messageWrapper.Message.ToString());

                if (GameSetup.Instance.Players[0].Guid != Guid.Empty)
                {
                    var joiningPlayer = new Player { Guid = guid, Name = "Remote" };
                    GameSetup.Instance.AddPlayer(joiningPlayer);
                }
                else
                {
                    GameSetup.Instance.PlayerGuid = guid;
                    GameSetup.Instance.Players[0].Guid = guid;
                    GameSetup.Instance.Players[0].Name = "Local";
                }
            }
            else if (messageWrapper.Type == MessageType.GameGuid)
            {
                Debug.Log("(game guid) : " + messageWrapper.Message.ToString());

                var guid = Guid.Parse(messageWrapper.Message.ToString());
                GameSetup.Instance.GameGuid = guid;
            }
            else if (messageWrapper.Type == MessageType.String)
            {
                Debug.Log("(string) : " + messageWrapper.Message);
            }
            else if(messageWrapper.Type == MessageType.LobbyInfo)
            {
                Debug.Log("(lobbyInfo) : " + messageWrapper.Message);
                var lobbyInfo = JsonConvert.DeserializeObject<LobbyInfo>(messageWrapper.Message.ToString());

                GameSetup.Instance.LobbyPlayerCount = lobbyInfo.LobbyPlayerCount;
                GameSetup.Instance.ServerGames = lobbyInfo.ServerGames;
                
            }
            else if(messageWrapper.Type == MessageType.Dart)
            {
                Debug.Log("(dart) : " + messageWrapper.Message);
                var onlineDart = JsonConvert.DeserializeObject<OnlineDart>(messageWrapper.Message.ToString());

                GameController.Instance.ThrowOnlineDart(onlineDart);
            }
            else if(messageWrapper.Type == MessageType.EndTurn)
            {
                Debug.Log("(end turn) : " + messageWrapper.Message);
                var playerGuid = Guid.Parse(messageWrapper.Message.ToString());
                GameController.Instance.SetTurnOver(playerGuid);
            }
            else if(messageWrapper.Type == MessageType.Disconnect)
            {
                Debug.Log("(disconnect) : " + messageWrapper.Message);
                GameController.Instance.OpponentDisconnected();
            }


            ReceiveAsync(_socket);
        }

        public void SendHostGame()
        {
            var mw = new MessageWrapper { Type = MessageType.HostGame, Message = GameSetup.Instance.GameName };
            SendObject<MessageWrapper>(_writer, mw);
        }

        public void SendJoinGame(Guid gameGuid)
        {
            var mw = new MessageWrapper { Type = MessageType.JoinGame, Message = gameGuid };
            SendObject<MessageWrapper>(_writer, mw);
        }

        public void SendDart(Dart dart)
        {
            var onlineDart = new OnlineDart
            {
                GameGuid = GameSetup.Instance.GameGuid,
                PlayerGuid = GameSetup.Instance.PlayerGuid,
                Dart = dart
            };

            var mw = new MessageWrapper { Type = MessageType.Dart, Message = onlineDart };
            SendObject<MessageWrapper>(_writer, mw);
        }

        public void SendEndTurn()
        {
            var mw = new MessageWrapper { Type = MessageType.EndTurn, Message = GameSetup.Instance.PlayerGuid };
            SendObject<MessageWrapper>(_writer, mw);
        }

        public void SendDisconnect()
        {
            var mw = new MessageWrapper { Type = MessageType.Disconnect, Message = null };
            SendObject<MessageWrapper>(_writer, mw);
        }

        void SendObject<T>(StreamWriter s, T o)
        {
            if (!_socketReady) return;

            string message = "";
            try
            {
                message = JsonConvert.SerializeObject(o);
            }
            catch(Exception e)
            {
                Debug.Log("JSON Convert exception : " + e.Message);
            }

            Debug.Log("Send object message : " + message);
            s.WriteLine(message);
            s.Flush();
        }

        T ReadObject<T>(StreamReader r)
        {
            var line = r.ReadLine();
            if (line == null) return default(T);
            Debug.Log("Read object line : " + line);

            T message = default(T);

            try
            {
                message = JsonConvert.DeserializeObject<T>(line);
            }
            catch(Exception e)
            {
                Debug.Log("JSON read exception : " + e.Message);
            }

            return message;
        }


        private void CloseSocket()
        {
            if (!_socketReady) return;

            SendDisconnect();
            _writer.Close();
            _reader.Close();
            _socket.Close();
            _socketReady = false;
        }

        private void OnApplicationQuit()
        {
            CloseSocket();
        }

        private void OnDisable()
        {
            CloseSocket();
        }

        private void OnDestroy()
        {
            CloseSocket();
        }

    }
}
