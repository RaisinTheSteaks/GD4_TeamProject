using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PlayerType {Player, Spectator }

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //instance
    public static NetworkManager instance;
    public List<string> spectator;

    private void Awake()
    {
        //if an instance already exist and it's not this one -  destroy us
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            //set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        spectator = new List<string>();
    }

    public void CreateRoom(string roomName)
    {
        if(PhotonNetwork.NickName != "")
            PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnCreatedRoom()
    {

    }

    public void JoinRoom (string roomName)
    {
        //limiting player count into 2 players per room
        //if(PhotonNetwork.PlayerList.Length <= 2)
        //{
        if (PhotonNetwork.NickName != "")
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(roomName);
        }
            
        //}
    }


    [PunRPC]
    public void ChangeScene (string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

};
