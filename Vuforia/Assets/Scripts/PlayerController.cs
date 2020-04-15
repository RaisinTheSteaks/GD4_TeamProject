using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.SceneManagement;

enum PowerUp
{
    DoubleDamage,
    StopTime,
    SwapPosition,
    Random
}

public class PlayerController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;
    public Player photonPlayer;
    public bool Turn;
    private Button EndTurnButton;
    public float timer;
    public bool endTurnPressed;
    public Text endTurnMessage;
    public GameObject endTurnMessageImage;
    public GameObject botSymbol;
    public int actionCount = 0;

    public HexGrid grid;
    public bool timeStop;
    public bool timeStopUsed = false;
    public bool doubleDamageUsed = false;
    public bool randomUsed = false;
    public float timeStopTimer = 0.0f;

    public GameObject popUp;
    public GameObject pauseScreen;
    public bool pause = false;

    public GameObject action1;
    public GameObject action2;

    public bool botHasMoved;

    //GameOver
    public bool hasChildren = true;
    public bool winner = true;
    private GameObject endScreen;
    public bool endGame = false;
    public float playerClock;
    public Text endText;
    public bool troopAbility;
    private ActionPanel attackBubbleChat;
    private GameObject operatorObject;


    [PunRPC]
    public void Initialize(Player player)
    {
        //transform.SetParent(GameManager.instance.imageTarget.transform);
        transform.SetParent(GameManager.instance.grid.transform, false);

        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players[id - 1] = this;


        if (player.IsMasterClient)
            setTurn(true); //if the player is the first in the list, then the game starts with them being the active player
        else
            setTurn(false);

        foreach (Transform child in transform)
        {
            BotController botScript = child.GetComponent<BotController>();
            botScript.InitializeBot();
        }
        EndTurnButton = GetComponent<Button>();
        grid = GameManager.instance.grid;


        //GameOver
        AssignClock(player);

    }

    private void Awake()
    {
        popUp = GameObject.Find("PopUp");
        pauseScreen = GameObject.Find("PauseMenu");
        
    }


    private void Start()
    {
        transform.name = photonPlayer.NickName;

        endScreen = GameObject.Find("EndScreen");
        endText = endScreen.transform.Find("Text").GetComponent<Text>();
        EndTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnMessageImage = GameObject.Find("EndTurnMessage");
        endTurnMessage = endTurnMessageImage.transform.Find("Text").GetComponent<Text>();
        botSymbol = GameObject.Find("Symbol");

        action1 = GameObject.Find("ActionCount").transform.Find("ActionCount1").gameObject;
        action2 = GameObject.Find("ActionCount").transform.Find("ActionCount2").gameObject;
        endTurnMessageImage.SetActive(false);
        endTurnPressed = false;
        timeStop = false;

        botSymbol = GameObject.Find("Symbol");
        popUp.SetActive(false);
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
        troopAbility = false;
        //It's the little cute robot on the right
        operatorObject = GameObject.Find("Operator").gameObject;
        GameObject attackDebug = GameObject.Find("AttackDebug").gameObject;
        attackBubbleChat = new ActionPanel(attackDebug, attackDebug.transform.position, attackDebug.transform.localScale, false, 5);
    }

    private void Update()
    {
        //if(attackBubbleChat != null)
        //{
            ShowAttackBubble();

        //}
        if(operatorObject)
            attackBubbleChat.transition(operatorObject.transform.position);
        AssignClock(photonPlayer);
        CheckTurn(); 
        if(action1 && action2)
            UpdateActionCount();
        StartTimeStop();

        if (Turn && !pause)
        {
            SelectCharacter();
        }


        if (endTurnPressed && Turn && !pause)
        {
            endTurnMessage.text = "End Turn \n" + (3 - (int)timer);
            timer += Time.deltaTime;

            if (timer >= 3)
            {

                EndTurn();
                ResetEndTurnButton();
            }
        }

        foreach (Transform bot in transform)
        {
            if (photonPlayer == PhotonNetwork.LocalPlayer)
            {
                if (bot.GetComponent<BotController>().isSelected)
                {
                    botSymbol.SetActive(true);
                    break;
                }

                botSymbol.SetActive(false);
            }


        }

        if (playerClock <= 1)
        {
            winner = false;
            GameManager.instance.photonView.RPC("EndGame", RpcTarget.AllBuffered);
        }
        if (endGame)
        {
            WinScreens();
        }

    }

    public void ShowAttackBubble()
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            if(child.transform.GetComponent<BotController>().showBubble)
            {
                this.attackBubbleChat.show = true;
                count++;
            }
        }

        if(count == 0 && photonPlayer.IsLocal)
        {
            this.attackBubbleChat.show = false;
        }
        
    }

    private void UpdateActionCount()
    {
        if (actionCount == 0)
        {
            action1.GetComponent<Image>().color = Color.white;
            action2.GetComponent<Image>().color = Color.white;
        }
        if (actionCount == 1)
        {
            action1.GetComponent<Image>().color = Color.grey;

        }
        else if(actionCount == 2)
        {
            action1.GetComponent<Image>().color = Color.grey;
            action2.GetComponent<Image>().color = Color.grey;
            botHasMoved = true;
        }
    }

    private void AssignClock(Player player)
    {
        if (player.IsMasterClient)
            playerClock = GameObject.Find("Timer").GetComponent<ChessClockController>().player1Time; 
        else
            playerClock = GameObject.Find("Timer").GetComponent<ChessClockController>().player2Time;
    }
    private void SelectCharacter()                                                   //Selects a character by drawing a raycast to where the mouse is pointing
    {                                                                                //If it is currently the players turn & the object they click on is a child of the player
        if (Input.GetMouseButtonDown(0))                                             //then the child is able to perform its functions in the game like moving, shooting etc.
        {                                                                            //It then foes through a list of all the players children and if these children aren't the selected object then it 
                                                                                     //sets them as unselected.
            if (photonPlayer == PhotonNetwork.LocalPlayer)
            {
                if (Turn)
                {
                    //Debug.Log("Selecting");
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.parent == null)
                        {
                            Debug.Log("Cannot be selected");
                        }
                        else if (hit.transform.parent == transform)
                        {
                            foreach (Transform child in hit.transform.parent)
                            {
                                if (child != hit.transform)
                                    child.transform.GetComponent<BotController>().isSelected = false;
                            }
                            hit.transform.GetComponent<BotController>().isSelected = true;
                        }
                    }
                }
            }

        }
    }

    public void RandomPowerups()
    {        
       int powerUpCount = (int)PowerUp.Random;
       int rng = UnityEngine.Random.Range(0, powerUpCount);
       PowerUp choice = (PowerUp)rng;
        choice = PowerUp.SwapPosition;
       switch (choice)
       {
            case PowerUp.DoubleDamage:
                 if(doubleDamageUsed)                   
                   doubleDamageUsed = false;
                 DoubleDamage();
            break;

            case PowerUp.StopTime:
                 if (timeStopUsed)
                    timeStopUsed = false;
                 StopTime();
                 break;
            case PowerUp.SwapPosition:
                 SwapBotPos();
                 break;

       }


    }

    public void DoubleDamage()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<BotController>().DoubleDamage();
        }

    }

    public void StopTime()
    {
        timeStop = true;
        photonView.RPC("SetClockState", RpcTarget.All, false);
    }

    public void SwapBotPos()
    {
        List<Unit> childs = new List<Unit>();
        foreach (Transform child in transform)
        {
            childs.Add(child.gameObject.GetComponent<Unit>());
        }

        HexCell temp = childs[0].Location;
        childs[0].Location = childs[1].Location;
        childs[1].Location = temp;

        childs[0].Location.unit = childs[0];
        childs[1].Location.unit = childs[1];
    }

    public void StartTimeStop()
    {
        if(timeStop)
        {
            timeStopTimer += Time.deltaTime;

            if (timeStopTimer >= 10)
            {
                photonView.RPC("SetClockState", RpcTarget.All, true);
                timeStop = false;
                timeStopTimer = 0;
            }
        }

    }

    [PunRPC]
    public void SetClockState(bool state)
    {
        GameManager.instance.clocks.GetComponent<ChessClockController>().startClock = state;
    }

    public void CheckTurn()
    {
        if (photonPlayer == PhotonNetwork.LocalPlayer)
        {
            foreach (Transform text in EndTurnButton.transform)
            {
                text.gameObject.SetActive(Turn);
            }
            EndTurnButton.enabled = Turn;

            if (!Turn)
            {
                EndTurnButton.gameObject.GetComponent<Image>().material = Resources.Load("HUD/EndTurnDisabled", typeof(Material)) as Material;

            }
            else
            {
                EndTurnButton.gameObject.GetComponent<Image>().material = Resources.Load("HUD/EndTurn", typeof(Material)) as Material;
            }

        }

    }

    public void OnEndTurnButtonPressed()
    {
        //print("pressing down");
        if (Turn)
        {
            endTurnMessageImage.SetActive(true);
            endTurnPressed = true;
           
        }

    }

    public void EndTurn()
    {
        GameManager.instance.photonView.RPC("ChangeActivePlayer", RpcTarget.AllBuffered);
        actionCount = 0;
        if (timeStop)
        {
            photonView.RPC("SetClockState", RpcTarget.All, true);

        }
    }

    public void OnEndTurnRelease()
    {
        // print("releasing");
        ResetEndTurnButton();
    }

    public void ResetEndTurnButton()
    {
        endTurnMessageImage.SetActive(false);
        //endTurnMessage.gameObject.SetActive(false);
        endTurnPressed = false;
        timer = 0;
    }

    public void setTurn(bool isActive)
    {
        Turn = isActive;
        foreach (Transform child in transform)
        {
            BotController botScript = child.GetComponent<BotController>();
            if (!Turn)
            {
                grid.hexesTravelled = 0;
            }
        }
    }


    public void Pause()
    {
        pause = true;
        pauseScreen.SetActive(true);
        action1.SetActive(false);
        action2.SetActive(false);
        foreach (Transform child in transform)
        {
            BotController botScript = child.GetComponent<BotController>();
            botScript.pause = true;
        }
    }
    public void Resume()
    {
        pause = false;
        action1.SetActive(true);
        action2.SetActive(true);
        pauseScreen.SetActive(false);
        foreach (Transform child in transform)
        {
            BotController botScript = child.GetComponent<BotController>();
            botScript.pause = false;
        }
    }

    public void CheckChildren()
    {
        
        foreach (Transform bot in transform)
        {
            if (bot.GetComponent<BotController>().health <= 0)
            {
                hasChildren = false;
            }
            else
            {
                hasChildren = true;
                break;
            }
        }
        if (hasChildren)
        {
            Debug.Log("I HAVE CHILDREN");
        }
        else if (!hasChildren)
        {
            Debug.Log("No children");
            winner = false;
            GameManager.instance.photonView.RPC("EndGame", RpcTarget.AllBuffered);
        }
    }

    public void WinScreens()
    {
        if (winner)
        {
            DisplayWinScreen();
        }
        else if (!winner)
        {
            DisplayLoseScreen();
        }
        StartCoroutine(WaitForSceneLoad());
    }

    private void DisplayWinScreen()
    {
        endScreen.SetActive(true);
        endText.text = "You Win";
    }

    public void DisplayLoseScreen()
    {
        endScreen.SetActive(true);
        endText.text = "You Lose";
    }

    private IEnumerator WaitForSceneLoad()
    {
      yield return new WaitForSeconds(3);
      SceneManager.LoadScene("Menu");

    }
}
