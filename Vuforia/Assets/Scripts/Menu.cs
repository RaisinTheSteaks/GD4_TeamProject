using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{


    [Header("Screens")]
    public static int screenCount = 5;
    public GameObject[] screens = new GameObject[screenCount];

    public GameObject mainScreen;
    public GameObject lobbyScreen;
    public GameObject createLobbyScreen;
    public GameObject listingScreen;
    public GameObject playGameScreen;
    public string sceneName;
    public string galleryScene;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;
    public TextMeshProUGUI roomListText;
    
    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;
    public TextMeshProUGUI roomNameText;
    public GameObject howToPlay;

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

    public void QuitGame()
    {
        Application.Quit();
    }

    void SetScreen(GameObject screen)
    {
        //Debug.Log("Setting Active Screen");
        if (screen.Equals(listingScreen))
        {
            mainScreen.SetActive(false);
            playGameScreen.SetActive(false);
            listingScreen.GetComponent<CanvasScaler>().scaleFactor = 1;
            listingScreen.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            //deactivate all screens
            foreach (GameObject sc in screens)
            {
                if(sc != listingScreen)
                { 
                    sc.SetActive(false);
                }
            }
            //The Listing Screen needs to stay active throughout runtime, so it's canvas gets enabled/disabled

            listingScreen.GetComponent<Canvas>().enabled = false;
            screen.SetActive(true);
        }
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

    public void ReturnToMenu()
    {
        SetScreen(mainScreen);
    }

    #region Buttons

    public void OnCreateGameButton()
    {
        SetScreen(createLobbyScreen);
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

    public void OnGalleryModeButton()
    {
        SceneManager.LoadScene(galleryScene, LoadSceneMode.Single);
    }
    
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
    
    public void OnStartGameButton()
    {
        //Scene that will be loaded is Duplicate instead of MasterScene

        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, sceneName);
    }
    
    public void OnPlayGameButton()
    {
        SetScreen(playGameScreen);
    }

    #endregion

    #region Player Joining & Leaving Room
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
    
    #endregion
    
    #region How To play
    public void CloseHowToPlay()
    {
        howToPlay.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        howToPlay.SetActive(true);
    }
    #endregion
    



}
