using Assets.Scripts.GameState;
using Assets.Scripts.GameState.DartMap;
using Assets.Scripts.GameState.Games;
using Assets.Scripts.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// 
/// View on top of the Game
///
public class GameController : MonoBehaviour
{
    #region Fields
    private bool _inTransition;
    private string _networkUpdate;
    #endregion


    #region GUI Properties
    public static GameController Instance { get; set; }
    public List<Text> PlayerTexts;
    public Text Dart1Text;
    public Text Dart2Text;
    public Text Dart3Text;
    public Text TurnTransitionText;
    public Text NetworkUpdateText;
    #endregion


    #region Game state Properties
    public Game Game;
    #endregion

    #region Unity lifecycle Methods
    private void Awake()
    {
        Instance = this;
        Game = GameSetup.Instance.GetGame();

        PlayerTexts = new List<Text>();

        for (int i = 0; i < GameSetup.Instance.Players.Count; i++)
        {
            var textboxName = "Player" + (i + 1) + "Text";
            var textbox = GameObject.Find(textboxName).GetComponent<Text>();
            PlayerTexts.Add(textbox);
        }

        RenderPlayerInfo();
    }

    void Update()
    {
        if (!_inTransition)
        {
            if (Input.anyKeyDown)
            {
                HandleInput();
            }

            ///
            /// Transitions - order seems to matter
            /// 

            if (Game.IsWin)
            {
                StartCoroutine(WinTransition());
            }

            if (Game.IsBust && !_inTransition)
            {
                StartCoroutine(BustTransition());
            }

            if (Game.TurnOver && !_inTransition)
            {
                StartCoroutine(TurnTransition());
            }
        }

        Render();
    }
    #endregion


    #region Game state methods
    private void HandleInput()
    {
        var inputValue = new KeyCode();
        Dart dart = default(Dart);

        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                inputValue = kcode;
                Debug.Log(inputValue.ToString());

                // Get Dart from DartMap
                try
                {
                    dart = DartMapKeyboard.GetMark[inputValue];
                    Debug.Log(dart.ToString());
                }
                catch (KeyNotFoundException ke) { }

                if (dart == null) return;

                if (GameSetup.Instance.IsOnline)
                {
                    GameSetup.Instance.Client.SendDart(dart);
                }
                else
                {
                    // Apply game rules to dart to determine value
                    Game.ThrowDart(dart);
                }
            }
        }
    }

    public void SetTurnOver(Guid playerGuid)
    {
        if (GameSetup.Instance.IsOnline)
        {
            if (Game.ActivePlayer.Guid == playerGuid)
            {
                Game.TurnOver = true;
            }
        }
    }

    public void SetTurnOver()
    {
        if (GameSetup.Instance.IsOnline)
        {
            GameSetup.Instance.Client.SendEndTurn();
        }
        else
        {
            Game.TurnOver = true;
        }
    }

    public void ThrowOnlineDart(OnlineDart onlineDart)
    {
        if(Game.ActivePlayer.Guid == onlineDart.PlayerGuid)
        {
            Game.ThrowDart(onlineDart.Dart);
        }
    }

    public void ThrowDart(string segment)
    {
        var dart = DartMapBoard10x10.GetMark[segment];
        Game.ThrowDart(dart);
    }

    public void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpponentDisconnected()
    {
        _networkUpdate = "Opponent Disonnected";
    }
    #endregion


    #region Rendering methods
    private void Render()
    {
        RenderDartIndicator();
        RenderPlayerInfo();

        NetworkUpdateText.text = _networkUpdate;
    }

    private void RenderPlayerInfo()
    {
        for (int i = 0; i < Game.Players.Count; i++)
        {
            PlayerTexts[i].text = Game.Players[i].ToString();
            PlayerTexts[i].color = Color.black;
        }

        PlayerTexts[Game.ActivePlayerIndex].color = Color.yellow;
    }

    private void RenderDartIndicator()
    {
        Dart1Text.text = Game.DartsThisTurn.ElementAt(0) != null ? Game.DartsThisTurn.ElementAt(0).ToString() : "Dart";
        Dart2Text.text = Game.DartsThisTurn.ElementAt(1) != null ? Game.DartsThisTurn.ElementAt(1).ToString() : "Dart";
        Dart3Text.text = Game.DartsThisTurn.ElementAt(2) != null ? Game.DartsThisTurn.ElementAt(2).ToString() : "Dart";
    }
    #endregion




    #region Transitions
    IEnumerator TurnTransition()
    {
        // Do immediately when called
        _inTransition = true;
        Debug.Log("Turn transition START");
        TurnTransitionText.text = Game.NextPlayer.Name + " is up next";

        // Yield to game loop for # seconds
        yield return new WaitForSecondsRealtime(3);

        // Return and execute from here
        TurnTransitionText.text = "";
        Time.timeScale = 1;

        Game.EndTurn();

        _inTransition = false;
        Debug.Log("Turn transition END");
    }

    IEnumerator WinTransition()
    {
        // Do immediately when called
        _inTransition = true;
        Debug.Log("Win transition START");
        TurnTransitionText.text = Game.ActivePlayer.Name + " WINS!";

        // Yield to game loop for # seconds
        yield return new WaitForSecondsRealtime(3);

        // Return and execute from here
        TurnTransitionText.text = "";
        Time.timeScale = 1;

        _inTransition = false;
        Debug.Log("Win transition END");

        Game.EndTurn();
        SceneManager.LoadScene("GameStats");
    }

    IEnumerator BustTransition()
    {
        // Do immediately when called
        _inTransition = true;
        Debug.Log("Bust transition START");
        TurnTransitionText.text = Game.ActivePlayer.Name + " BUSTED!";

        // Yield to game loop for # seconds
        yield return new WaitForSecondsRealtime(3);

        // Return and execute from here
        TurnTransitionText.text = "";
        Time.timeScale = 1;

        Game.EndTurn();

        _inTransition = false;
        Debug.Log("Bust transition END");
    }

    IEnumerator Wait()
    {
        _inTransition = true;
        yield return new WaitForSecondsRealtime(3);
        _inTransition = false;
    }
    #endregion
}
