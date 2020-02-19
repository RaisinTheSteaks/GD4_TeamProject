using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

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

    public HexGrid grid;

    public GameObject popUp;


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

        

    }


    private void Awake()
    {
        popUp = GameObject.Find("PopUp");

    }


    private void Start()
    {
        transform.name = photonPlayer.NickName;

        EndTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnMessageImage = GameObject.Find("EndTurnMessage");
        endTurnMessage = endTurnMessageImage.transform.Find("Text").GetComponent<Text>();
        botSymbol = GameObject.Find("Symbol");
        
        endTurnMessageImage.SetActive(false);
        endTurnPressed = false;

        botSymbol = GameObject.Find("Symbol");
        popUp.SetActive(false);

    }

    private void Update()
    {
        checkTurn();
        if (Turn)
        {
            SelectCharacter();
        }

        
        if(endTurnPressed && Turn)
        {
            endTurnMessage.text = "End Turn \n" + (3 - (int)timer);
            timer += Time.deltaTime;
            
            if (timer >= 3)
            {
                
                EndTurn();
                ResetEndTurnButton();
            }
        }

        foreach(Transform bot in transform)
        {
            if(photonPlayer == PhotonNetwork.LocalPlayer)
            {
                if (bot.GetComponent<BotController>().isSelected)
                {
                    botSymbol.SetActive(true);
                    break;
                }

                botSymbol.SetActive(false);
            }
            

        }
        
    }


    //Selects a character by drawing a raycast to where the mouse is pointing
    //If it is currently the players turn & the object they click on is a child of the player
    //then the child is able to perform its functions in the game like moving, shooting etc.
    //It then foes through a list of all the players children and if these children aren't the selected object then it 
    //sets them as unselected.
    private void SelectCharacter()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (photonPlayer == PhotonNetwork.LocalPlayer)
            {
                if (Turn)
                {
                    Debug.Log("Selecting");
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
                            botSymbol.GetComponent<RawImage>().material = hit.transform.GetComponent<BotController>().symbol;
                        }
                    }
                }
            }
            
        }
    }


    public void checkTurn()
    {
        if (photonPlayer == PhotonNetwork.LocalPlayer)
        //EndTurnButton.interactable = Turn;
        {
            foreach(Transform text in EndTurnButton.transform)
            {
                text.gameObject.SetActive(Turn);
            }
            EndTurnButton.enabled = Turn;
            
            if (!Turn)
            {
                EndTurnButton.gameObject.GetComponent<Image>().material = Resources.Load("EndTurnDisabled", typeof(Material)) as Material;

            }
            else
            {
                EndTurnButton.gameObject.GetComponent<Image>().material = Resources.Load("EndTurn", typeof(Material)) as Material;
            }

        }
           
    }

    public void OnEndTurnButtonPressed()
    {
        //print("pressing down");
        if(Turn)
        {
            endTurnMessageImage.SetActive(true);
            endTurnPressed = true;
        }
        
    }

    public void EndTurn()
    {
        GameManager.instance.photonView.RPC("ChangeActivePlayer", RpcTarget.AllBuffered);

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
                botScript.SelectedStatus.SetText(Turn.ToString());
                grid.hexesTravelled = 0;
            }
        }
    }
}
