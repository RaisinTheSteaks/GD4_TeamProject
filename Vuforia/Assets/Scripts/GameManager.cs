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
    public HexGrid grid;
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
            //Hex points is these 2 points
        }
        else
        {
            index = 1;
        }
            
        
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[index].position, Quaternion.identity);


        ////initialize the player
        PlayerController playerScript = playerObject.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

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
    public void ChangeActivePlayer()
    {
        foreach (PlayerController player in players)
        {
          player.Turn = !player.Turn;
          player.setTurn(player.Turn);
          foreach(Transform child in player.gameObject.transform)
          {
                child.GetComponent<BotController>().isSelected = false;
                child.GetComponent<BotController>().attackingMode = false;
          }

        }
    }
}
