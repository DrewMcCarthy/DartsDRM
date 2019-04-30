using Assets.Scripts.GameState.Games;
using Assets.Scripts.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameState
{
    public class GameSetup
    {
        #region Singleton Implementation
        private static GameSetup _instance = null;
        private GameSetup(){}
        public static GameSetup Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameSetup();
                }
                return _instance;
            }
        }

        public static void Recycle()
        {
            _instance = null;
        }
        #endregion

        public event EventHandler OnPlayerAdded;
        public event EventHandler OnPlayerCountReached;
        public List<Player> Players = new List<Player>();
        public string GameName { get; set; }
        public int PlayerCount { get; set; }
        public bool IsOnline { get; set; }
        public int LobbyPlayerCount { get; set; }
        public int LobbyGameCount
        {
            get
            {
                if(ServerGames != null)
                {
                    return ServerGames.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        public List<ServerGame> ServerGames { get; set; }
        public Guid GameGuid { get; set; }
        public Guid PlayerGuid { get; set; }
        public Client Client { get; set; }
        public bool DartboardConnected { get; set; }

        public void AddPlayer(Player player)
        {
            if (IsOnline)
            {
                // If hosting, add new players to end of list
                // else add the host to the beginning. First player in list will go first
                if (Client.IsHost)
                {
                    Players.Add(player);
                }
                else
                {
                    Players.Insert(0, player);
                }
            }
            else
            {
                Players.Add(player);
            }

            OnPlayerAdded(this, EventArgs.Empty);

            if (Players.Count == PlayerCount)
            {
                Debug.Log("First Player : " + Players[0].Guid.ToString());
                Debug.Log("Second Player : " + Players[1].Guid.ToString());

                OnPlayerCountReached(this, EventArgs.Empty);
            }
        }


        public Game GetGame()
        {
            Debug.Log("Game Name : " + GameName);

            if (GameName == "301")
            {
                return new ZeroOne(75); 
            }
            //else if (GameName == "Cricket")
            //{
            //    return new Cricket();
            //}
            else
            {
                throw new Exception("Invalid Game");
            }
        }
    }
}
