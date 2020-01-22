using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject imageTarget;

    [Header("Stats")]
    public bool gameEnded = false;
    public TextMeshProUGUI pingUI;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public BotController[] bots;
    public PlayerController[] players;
    private int playersInGame;
    private List<int> pickedSpawnIndex;

    [Header("Targets")]
    public GameObject selectedTarget;


    //instance
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pickedSpawnIndex = new List<int>();
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        bots = new BotController[players.Length * 2];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        
    }

    private void Update()
    {
       
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    void SpawnPlayer()
    {

        int index;
        if (PhotonNetwork.IsMasterClient)
        {
            index = 0;
        }
        else
        {
            index = 1;
        }
            
        
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[index].position, Quaternion.identity);


        ////initialize the player
        PlayerController playerScript = playerObject.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

        players[0].Turn = true;

    }


    public PlayerController GetPlayer (int playerID)
    {
        return players.First(x => x.id == playerID);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    public PlayerController GetPlayer(string nickname)
    {
        return players.First(x => x.transform.name == nickname);
    }

    [PunRPC]
    public void ChangeActivePlayer(int playerId, bool initialActive)
    {
        //if this is the first time the active player is being decided then the first player in the players list becomes active
        if (initialActive)
        {
            GetPlayer(playerId).setActive(true);
            foreach (PlayerController player in players)
            {
                if (player.id != playerId)
                    player.setActive(false);
                else
                    player.setActive(true);
            }
        }

        else if (!initialActive)
        {
            foreach (PlayerController player in players)
            {
                if (player.id == playerId)
                    player.setActive(true);
                else
                    player.setActive(false);
            }
        }

        //if this isn't the first time the active player is being decided, then set the active player to false.
        //if (!initialActive)
        //GetPlayer(activePlayer).setActive(false);

        // give the hat to the new player
        //activePlayer = playerId;
        //GetPlayer(playerId).setActive(true);


    }

}
