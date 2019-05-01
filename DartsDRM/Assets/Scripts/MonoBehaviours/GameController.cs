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
    public List<Text> PlayerNameTexts;
    public List<Text> PlayerGameScoreTexts;
    public List<Text> PlayerRoundScoreTexts;
    public Text Dart1Text;
    public Text Dart2Text;
    public Text Dart3Text;
    public Text TurnTransitionText;
    public Text NetworkUpdateText;
    public Color ActivePlayerTextColor;
    public Color InactivePlayerTextColor;
    #endregion


    #region Game state Properties
    public Game Game;
    #endregion

    #region Unity lifecycle Methods
    private void Awake()
    {
        Instance = this;
        Game = GameSetup.Instance.GetGame();

        PlayerGameScoreTexts = new List<Text>();
        PlayerRoundScoreTexts = new List<Text>();

        ActivePlayerTextColor = new Color(255f / 255f, 171f / 255f, 0f / 255f);
        InactivePlayerTextColor = new Color(0,0,0);

        for (int i = 0; i < GameSetup.Instance.Players.Count; i++)
        {
            // Find and add textboxes for Player names
            var nameTextboxName = "Player" + (i + 1) + "Name";
            var nameTextbox = GameObject.Find(nameTextboxName).GetComponent<Text>();
            PlayerNameTexts.Add(nameTextbox);

            // Find and add textboxes for GameScore
            var gameScoretextboxName = "Player" + (i + 1) + "GameScore";
            var gameScoretextbox = GameObject.Find(gameScoretextboxName).GetComponent<Text>();
            PlayerGameScoreTexts.Add(gameScoretextbox);

            // Find and add textboxes for RoundScore
            var roundScoretextboxName = "Player" + (i + 1) + "RoundScore";
            var roundScoretextbox = GameObject.Find(roundScoretextboxName).GetComponent<Text>();
            PlayerRoundScoreTexts.Add(roundScoretextbox);
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
        // Render values and set all players' color to inactive player color
        for (int i = 0; i < Game.Players.Count; i++)
        {
            PlayerNameTexts[i].color = InactivePlayerTextColor;

            PlayerGameScoreTexts[i].text = "gameScore: " + Game.Players[i].GameScore.ToString();
            PlayerGameScoreTexts[i].color = InactivePlayerTextColor;

            PlayerRoundScoreTexts[i].text = "roundScore: " + Game.Players[i].RoundScore.ToString();
            PlayerRoundScoreTexts[i].color = InactivePlayerTextColor;
        }

        // Update active player to active player color
        PlayerNameTexts[Game.ActivePlayerIndex].color = ActivePlayerTextColor;
        PlayerGameScoreTexts[Game.ActivePlayerIndex].color = ActivePlayerTextColor;
        PlayerRoundScoreTexts[Game.ActivePlayerIndex].color = ActivePlayerTextColor;
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
