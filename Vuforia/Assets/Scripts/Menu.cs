using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;
    public GameObject createLobbyScreen;
    public GameObject listingScreen;
    public string sceneName;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;
    public TextMeshProUGUI roomListText;



    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;
    public TextMeshProUGUI roomNameText;

    private bool joinAsSpectator = false;

    private void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;

    }
    private void Awake()
    {
        listingScreen.GetComponent<CanvasScaler>().scaleFactor = 0.001f;
    }


    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;

        PhotonNetwork.JoinLobby();
    }

    void SetScreen(GameObject screen)
    {
        if (screen.Equals(listingScreen))
        {
            mainScreen.SetActive(false);
            listingScreen.GetComponent<CanvasScaler>().scaleFactor = 1;
        }
        else
        {
            //deactivate all screen
            mainScreen.SetActive(false);
            lobbyScreen.SetActive(false);
            createLobbyScreen.SetActive(false);
            listingScreen.GetComponent<CanvasScaler>().scaleFactor = 0.01f;
            Debug.Log(listingScreen.GetComponent<CanvasScaler>().scaleFactor);
            Debug.Log("FUCKIUNG UNITY");
            //enable requested scene
            screen.SetActive(true);
        }
    }

    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        if (roomNameInput.text != "")
        {
            NetworkManager.instance.CreateRoom(roomNameInput.text);
            roomNameText.text = roomNameInput.text;
        }

    }

    public void OnJoinGameButton()
    {
        SetScreen(listingScreen);
    }

    public void OnJoinRoomButton(Text roomNameInput)
    {

        NetworkManager.instance.JoinRoom(roomNameInput.text);
        roomNameText.text = roomNameInput.text;
    }

    public void OnJoinRoomAsSpectatorButton(Text roomNameInput)
    {

        NetworkManager.instance.JoinRoom(roomNameInput.text);
        roomNameText.text = roomNameInput.text;
        joinAsSpectator = true;
    }

    [PunRPC]
    public void AddSpectator(string nickname)
    {
        NetworkManager.instance.spectator.Add(nickname);
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            if (player.NickName == PhotonNetwork.NickName)
            {
                OnLeaveLobbyButton();
                return;
            }
        }


        SetScreen(lobbyScreen);

        //tell all players to update the lobby screen
        if (joinAsSpectator)
        {
            photonView.RPC("AddSpectator", RpcTarget.All, PhotonNetwork.NickName);
        }

        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //we dont use RPC like when we join the lobby
        //that is because OnJoinedRoom is only called for the client who just joined the room
        //OnPlayerLeftRoom gets called for all clients in the room, so we don't need RPC
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient)
                playerListText.text += player.NickName + " (Host) \n";
            else if (NetworkManager.instance.spectator.Contains(player.NickName))
                playerListText.text += player.NickName + " (Spectator)\n";
            else
                playerListText.text += player.NickName + "\n";
        }

        //only host can start the game
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;

        //roomListText.text = PhotonNetwork.CloudRegion;

    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void CreateGame()
    {
        SetScreen(createLobbyScreen);
    }

    public void ReturnToMenu()
    {
        SetScreen(mainScreen);
    }
    public void OnStartGameButton()
    {
        //Scene that will be loaded is Duplicate instead of MasterScene

        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, sceneName);
    }






}
