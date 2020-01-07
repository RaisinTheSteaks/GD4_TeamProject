using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //instance
    public static NetworkManager instance;

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
    }

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        print("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }

    public void JoinRoom (string roomName)
    {
        //limiting player count into 2 players per room
        //if(PhotonNetwork.PlayerList.Length <= 2)
        //{
            PhotonNetwork.JoinRoom(roomName);
        //}
    }

    [PunRPC]
    public void ChangeScene (string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

};
