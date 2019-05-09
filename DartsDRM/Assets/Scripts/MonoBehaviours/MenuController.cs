using Assets.Scripts.GameState;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private int _inOutModeIndex;

    public Text InOutModeText;

    void OnEnable()
    {
        GameSetup.Recycle();

        GameSetup.Instance.OnPlayerAdded += OnPlayerAdded;
        GameSetup.Instance.OnPlayerCountReached += OnPlayerCountReached;

        DestroyNetworkObjects();
    }

    // Start is called before the first frame update
    void Start()
    {
        InOutModeText.text = Constants.InOutModes[_inOutModeIndex];
    }

    private static void DestroyNetworkObjects()
    {
        var networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if (networkObjects != null)
        {
            foreach (var net in networkObjects)
            {
                Destroy(net);
            }
        }
    }

    public void SetGameType(string gameType)
    {
        GameSetup.Instance.GameType = gameType;
        Debug.Log("Game Type : " + GameSetup.Instance.GameType);
    }

    public void SetGameName(string gameName)
    {
        GameSetup.Instance.GameName = gameName;
        Debug.Log("Game Name : " + GameSetup.Instance.GameName);
    }

    public void SetPlayerCount(int playerCount)
    {
        GameSetup.Instance.PlayerCount = playerCount;
    }

    public void SetIsOnline(bool isOnline)
    {
        GameSetup.Instance.IsOnline = isOnline;

        // For local games add the number of players selected in options to the game to start
        if (!isOnline)
        {
            for(int i = 1; i <= GameSetup.Instance.PlayerCount; i++)
            {
                GameSetup.Instance.AddPlayer(new Player() { Name = "Player " + i.ToString()});
            }
        }
    }

    public void SetInOutMode()
    {
        _inOutModeIndex++;
        if(_inOutModeIndex > Constants.InOutModes.Count - 1)
        {
            _inOutModeIndex = 0;
            Enum.TryParse("Active", out Constants.InMode myStatus);
        }

        InOutModeText.text = Constants.InOutModes[_inOutModeIndex];
    }

    public void OnPlayerAdded(object obj, EventArgs e)
    {
        Debug.Log("Player Added");
    }

    private void OnPlayerCountReached(object sender, EventArgs e)
    {
        SceneManager.LoadScene(GameSetup.Instance.GameType);
    }
}
