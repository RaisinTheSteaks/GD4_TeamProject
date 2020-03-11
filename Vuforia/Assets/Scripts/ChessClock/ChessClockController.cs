using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessClockController : MonoBehaviour
{
    public float player1Time = 300;
    public float player2Time = 300.1f; //player 2 clock is set as 300.1 because otherwise it spawns at 4:60.
    public Text player1Clock;
    public Text player2Clock;


    string player1Minutes = "00";
    string player1Seconds = "00";
    string player2Minutes = "00";
    string player2Seconds = "00";

    public bool player1Turn = true;
    public bool startClock;
    // Start is called before the first frame update
    void Start()
    {
        UpdatePlayer1Clock();
        UpdatePlayer2Clock();
    }

    // Update is called once per frame
    void Update()
    {
        if (startClock)
        {

            if (player1Turn)
            {
                UpdatePlayer1Clock();
            }
            else if (!player1Turn)
            {
                UpdatePlayer2Clock();
            }
        }
    }

    private void UpdatePlayer2Clock()
    {
        player2Time -= Time.deltaTime * 1;
        player2Minutes = Mathf.Floor((player2Time % 3600) / 60).ToString("00"); //60^2 = 3600
        player2Seconds = (player2Time % 60).ToString("00");
        player2Clock.text = player2Minutes + ":" + player2Seconds;
    }

    private void UpdatePlayer1Clock()
    {
        player1Time -= Time.deltaTime * 1;
        player1Minutes = Mathf.Floor((player1Time % 3600) / 60).ToString("00"); //60^2 = 3600
        player1Seconds = (player1Time % 60).ToString("00");
        player1Clock.text = player1Minutes + ":" + player1Seconds;
    }
    public void SwapClock()
    {
        player1Turn = !player1Turn;
    }


    
}

