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


    [PunRPC]
    public void Initialize(Player player)
    {
        transform.SetParent(GameManager.instance.imageTarget.transform);
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
    }

    private void Start()
    {
        if (photonPlayer == null)
        {
            Debug.Log("no networking");
        }
        else
        {
            transform.name = photonPlayer.NickName;
        }

        EndTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
    }

    private void Update()
    {
        checkTurn();
        if (Turn)
        {
            SelectCharacter();
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
            EndTurnButton.gameObject.SetActive(Turn);
    }

    public void OnEndTurnButton()
    {
        GameManager.instance.photonView.RPC("ChangeActivePlayer", RpcTarget.AllBuffered);
    }

    public void setTurn(bool isActive)
    {
        Turn = isActive;
        foreach (Transform child in transform)
        {
            BotController botScript = child.GetComponent<BotController>();
            if(!Turn)
            botScript.SelectedStatus.SetText(Turn.ToString());
        }
    }
}
