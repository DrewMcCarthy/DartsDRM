using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace DartServer
{
    public class Server
    {
        private List<ServerClient> _clients;
        private List<ServerClient> _disconnectedClients;
        public List<ServerGame> _serverGames;

        public int port = 8088;
        private TcpListener server;
        private bool serverStarted;

        public int IpAddress { get; private set; }

        public Server()
        {
            _clients = new List<ServerClient>();
            _disconnectedClients = new List<ServerClient>();
            _serverGames = new List<ServerGame>();
            var sg = new ServerGame();
            sg.GameGuid = Guid.NewGuid();
            _serverGames.Add(sg);

            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();

                StartListening();
                serverStarted = true;
                Console.WriteLine("Server started on port : " + port.ToString());

                MainLoop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Socket error : " + e.Message);
                Console.ReadLine();
            }
        }

        private void MainLoop()
        {
            while (true)
            {
                if (!serverStarted)
                {
                    return;
                }

                
                foreach (ServerClient c in _clients.ToList())
                {
                    // Is the client still connected?
                    if (!IsConnected(c.TcpClient))
                    {
                        c.TcpClient.Close();
                        c.Disconnected = true;
                        _disconnectedClients.Add(c);

                        Console.WriteLine("Client disconnected.");
                        continue;
                    }
                    // Check for message from the client
                    else
                    {
                        NetworkStream stream = c.TcpClient.GetStream();
                        if (stream.DataAvailable)
                        {
                            StreamReader reader = new StreamReader(stream, true);

                            var messageWrapper = ReadObject<MessageWrapper>(reader);

                            if (messageWrapper != null)
                            {
                                OnIncomingData(c, messageWrapper);
                            }
                        }
                    }
                }

                if (_disconnectedClients.Any(d => _clients.Contains(d)))
                {
                    _clients.RemoveAll(c => _disconnectedClients.Contains(c));
                    SendLobbyInfo(_clients);
                }
            }
        }



        private void OnIncomingData(ServerClient client, MessageWrapper mw)
        {
            if (mw.Type == MessageType.HostGame)
            {
                Console.WriteLine("(hostGame) : " + mw.Message);
                HandleHostGame(client);
            }
            else if(mw.Type == MessageType.JoinGame)
            {
                Console.WriteLine("(joinGame) : " + mw.Message);
                HandleJoinGame(client, mw);
            }
            else if (mw.Type == MessageType.Dart)
            {
                Console.WriteLine("(dart) : " + mw.Message);

                var gameClients = _clients.Where(c => c.GameGuid == client.GameGuid).ToList();
                Broadcast(mw, gameClients);
            }
            else if (mw.Type == MessageType.EndTurn)
            {
                Console.WriteLine("(end turn) : " + mw.Message);

                var gameClients = _clients.Where(c => c.GameGuid == client.GameGuid).ToList();
                Broadcast(mw, gameClients);
            }
            else if(mw.Type == MessageType.Disconnect)
            {
                Console.WriteLine("(disconnect) : " + mw.Message);


                var gameClients = _clients.Where(c => c.GameGuid == client.GameGuid).ToList();
                Broadcast(mw, gameClients);

                _serverGames.RemoveAll(g => g.GameGuid == client.GameGuid);
            }

        }

        private void HandleHostGame(ServerClient client)
        {
            var playerGuid = Guid.NewGuid();
            client.PlayerGuid = playerGuid;
            SendPlayerGuid(playerGuid, client);

            var gameGuid = Guid.NewGuid();
            client.GameGuid = gameGuid;

            Thread.Sleep(100);
            SendGameGuid(gameGuid, client);

            var sg = new ServerGame();
            sg.GameGuid = gameGuid;
            sg.PlayerGuids.Add(client.PlayerGuid);
            _serverGames.Add(sg);

            Thread.Sleep(100);
            SendLobbyInfo(_clients);
        }

        private void HandleJoinGame(ServerClient client, MessageWrapper mw)
        {
            var playerGuid = Guid.NewGuid();
            client.PlayerGuid = playerGuid;
            SendPlayerGuid(playerGuid, client);

            var gameGuid = Guid.Parse(mw.Message.ToString());
            client.GameGuid = gameGuid;

            var game = _serverGames.FirstOrDefault(sg => sg.GameGuid == gameGuid);
            game.PlayerGuids.Add(playerGuid);

            var hostGuid = game.PlayerGuids[0];
            var joinGuid = game.PlayerGuids[1];

            // Need to receive Player from joiner and send to host
            // and send host Player to joiner
            var hostClient = _clients.FirstOrDefault(c => c.PlayerGuid == hostGuid);
            var joinClient = _clients.FirstOrDefault(c => c.PlayerGuid == joinGuid);

            Thread.Sleep(100);
            SendPlayerGuid(hostGuid, joinClient);
            Thread.Sleep(100);
            SendPlayerGuid(joinGuid, hostClient);
        }

        private void Broadcast(MessageWrapper mw, List<ServerClient> clients)
        {
            foreach (var client in clients)
            {
                try
                {
                    if (client.TcpClient != null && client.TcpClient.Client != null && client.TcpClient.Connected)
                    {
                        StreamWriter writer = new StreamWriter(client.TcpClient.GetStream());
                        SendObject(writer, mw);
                    }                        
                }
                catch (Exception e)
                {
                    Console.WriteLine("Write error : " + e.Message + " to client " + client.Name);
                }
            }
        }

        private void Broadcast(MessageWrapper mw, ServerClient sc)
        {
            var scList = new List<ServerClient> { sc };
            Broadcast(mw, scList);
        }

        private void SendLobbyInfo(List<ServerClient> clients)
        {
            foreach (var client in clients)
            {
                try
                {
                    if (client.TcpClient != null && client.TcpClient.Client != null && client.TcpClient.Connected)
                    {
                        var lobbyInfo = new LobbyInfo { LobbyPlayerCount = _clients.Count, ServerGames = _serverGames };

                        var messageWrapper = new MessageWrapper
                        {
                            Type = MessageType.LobbyInfo,
                            Message = lobbyInfo
                        };

                        StreamWriter writer = new StreamWriter(client.TcpClient.GetStream());
                        SendObject(writer, messageWrapper);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Write error : " + e.Message + " to client " + client.Name);
                }
            }
        }

        private void SendPlayerGuid(Guid guid, ServerClient client)
        {
            try
            {
                if (client.TcpClient != null && client.TcpClient.Client != null && client.TcpClient.Connected)
                {
                    var messageWrapper = new MessageWrapper
                    {
                        Type = MessageType.PlayerGuid,
                        Message = guid
                    };

                    StreamWriter writer = new StreamWriter(client.TcpClient.GetStream());
                    SendObject(writer, messageWrapper);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Write error : " + e.Message + " to client " + client.Name);
            }
        }

        private void SendGameGuid(Guid guid, ServerClient client)
        {
            try
            {
                if (client.TcpClient != null && client.TcpClient.Client != null && client.TcpClient.Connected)
                {
                    var messageWrapper = new MessageWrapper
                    {
                        Type = MessageType.GameGuid,
                        Message = guid
                    };

                    StreamWriter writer = new StreamWriter(client.TcpClient.GetStream());
                    SendObject(writer, messageWrapper);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Write error : " + e.Message + " to client " + client.Name);
            }
        }

        private bool IsConnected(TcpClient c)
        {
            try
            {
                if (c != null && c.Client != null && c.Client.Connected)
                {
                    if (c.Client.Poll(0, SelectMode.SelectRead))
                    {
                        return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void StartListening()
        {
            server.BeginAcceptTcpClient(AcceptTcpClient, server);
        }

        private void AcceptTcpClient(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            var newClient = new ServerClient(listener.EndAcceptTcpClient(ar));
            _clients.Add(newClient);
            
            StartListening();

            // Send a message to everyone saying someone has connected
            SendLobbyInfo(_clients);
            Console.WriteLine("New player has joined lobby", _clients);
        }

        void SendObject<T>(StreamWriter s, T o)
        {
            s.WriteLine(JsonConvert.SerializeObject(o));
            s.Flush();
        }

        T ReadObject<T>(StreamReader r)
        {
            var line = r.ReadLine();
            if (line == null) return default(T);
            return JsonConvert.DeserializeObject<T>(line);
        }
    }


}
