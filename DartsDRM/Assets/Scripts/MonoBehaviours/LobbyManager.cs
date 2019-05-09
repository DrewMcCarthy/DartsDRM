using Assets.Scripts.GameState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.MonoBehaviours
{
    public class LobbyManager : MonoBehaviour
    {
        public Text GameTypeText;
        public Text TotalPlayersText;
        public GameObject buttonPrefab;
        public List<GameObject> CreatedButtons;
        public Transform menuPanel;
        public GameObject ClientPrefab;
        private Client _client;

        private async Task Awake()
        {
            _client = Instantiate(ClientPrefab).GetComponent<Client>();
            await _client.ConnectToServerAsync();

            CreatedButtons = new List<GameObject>();
        }

        private void Start()
        {
            GameTypeText.text = GameSetup.Instance.GameName + " Games: " + GameSetup.Instance.LobbyGameCount.ToString();
            TotalPlayersText.text = "Players Connected : " + GameSetup.Instance.LobbyPlayerCount.ToString();
            UpdateGameList();
        }

        private void Update()
        {
            GameTypeText.text = GameSetup.Instance.GameName + " Games: " + GameSetup.Instance.LobbyGameCount.ToString();
            TotalPlayersText.text = "Players Connected : " + GameSetup.Instance.LobbyPlayerCount.ToString();
        }

        public void HostGame()
        {
            _client.IsHost = true;
            GameSetup.Instance.AddPlayer(new Player());
            _client.SendHostGame();
        }

        public void JoinGame(Guid gameGuid)
        {
            Debug.Log("Button clicked guid : " + gameGuid);

            _client.IsHost = false;
            GameSetup.Instance.AddPlayer(new Player());
            GameSetup.Instance.GameGuid = gameGuid;
            _client.SendJoinGame(gameGuid);
        }

        public void UpdateGameList()
        {
            ClearButtons();

            if (GameSetup.Instance.ServerGames != null)
            {
                foreach (var sg in GameSetup.Instance.ServerGames)
                {
                    AddGameButton(sg.GameGuid);
                }
            }
        }

        public void Return()
        {
            SceneManager.LoadScene("MainMenu");
            // Destroy Client prefab and close socket
        }


        void AddGameButton(Guid gameGuid)
        {
            int posX = -220;
            int posY = (80 - (CreatedButtons.Count * 120));

            // Button from prefab
            GameObject button = (GameObject)Instantiate(buttonPrefab);

            // Set position
            var rectTrans = button.GetComponent<RectTransform>();
            rectTrans.localPosition = new Vector3(posX, posY);

            // Set text
            var btnText = button.GetComponentInChildren<Text>();
            btnText.text = gameGuid.ToString();

            // Add click event
            button.GetComponent<Button>().onClick.AddListener(() => { JoinGame(gameGuid); });

            // Set parent to Canvas
            button.transform.SetParent(menuPanel.transform, false);

            CreatedButtons.Add(button);
        }

        void ClearButtons()
        {
            for (int i = 0; i < CreatedButtons.Count; ++i)
            {
                if (CreatedButtons[i] != null)
                {
                    Destroy(CreatedButtons[i]);
                }
            }
            CreatedButtons.Clear();
        }

    }
}
