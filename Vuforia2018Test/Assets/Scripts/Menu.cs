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

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;
    public TextMeshProUGUI roomNameText;

    private void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    void SetScreen (GameObject screen)
    {
        //deactivate all screen
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        //enable requested scene
        screen.SetActive(true);
    }

    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
        roomNameText.text = roomNameInput.text; 
    }

    public void OnJoinRoomButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
        roomNameText.text = roomNameInput.text;
    }

    public void OnPlayerNameUpdate (TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);

        //tell all players to update the lobby screen
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

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient)
                playerListText.text += player.NickName + " (Host) \n";
            else
                playerListText.text += player.NickName + "\n";
        }

        //only host can start the game
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;

    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }




}
