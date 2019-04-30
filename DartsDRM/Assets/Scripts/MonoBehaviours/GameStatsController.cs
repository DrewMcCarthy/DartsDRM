using Assets.Scripts.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsController : MonoBehaviour
{
    public Text P1Ppd;
    public Text P2Ppd;
    public Text P1TopMarks;
    public Text P2TopMarks;

    // Start is called before the first frame update
    private void Start()
    {
        P1Ppd.text = GetPlayerPpd(0);
        P2Ppd.text = GetPlayerPpd(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string GetPlayerPpd(int playerIndex)
    {
        double result = 0.0;
        double dartValues = (double)GameSetup.Instance.Players[playerIndex].DartValues;
        double dartCount = (double)GameSetup.Instance.Players[playerIndex].DartCount;

        // Points per dart is sum Values/count of darts
        if (dartCount == 0.0)
        {
            result = 0.0;
        }
        else
        {
            result = dartValues / dartCount;
        }

        return result.ToString("0.00");
    }
}
