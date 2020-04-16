using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject imageTarget;
    public HexGrid grid;
    public HexMapController mapController;
    [Header("Stats")]
    public bool gameEnded = false;
    public TextMeshProUGUI pingUI;
    public GameObject PlayerHUD;

    [Header("Players")]
    public string playerOnePrefabLocation;
    public string playerTwoPrefabLocation;
    public HexCell[] spawnPoints;
    public BotController[] bots;
    public PlayerController[] players;
    private int playersInGame;
    private List<int> pickedSpawnIndex;
    public int playerSpeed = 3;
    [Header("Targets")]
    public GameObject selectedTarget;
    
    //Clock
    public GameObject clocks;



    //instance
    public static GameManager instance;

    private void Awake()
    {
        instance = this;


    }

    private void Start()
    {
        pickedSpawnIndex = new List<int>();
        players = new PlayerController[PhotonNetwork.PlayerList.Length - NetworkManager.instance.spectator.Count];
        bots = new BotController[players.Length * 2];
        foreach(string name in NetworkManager.instance.spectator)
        {
            Debug.Log(name);
        }
        Debug.Log(players.Length);
        if(!NetworkManager.instance.spectator.Contains(PhotonNetwork.NickName))
        {
            PlayerHUD.SetActive(true);
            clocks = GameObject.Find("Timer");
            photonView.RPC("ImInGame", RpcTarget.AllBuffered);
            clocks.GetComponent<ChessClockController>().startClock = true;

        }
        
        
        mapController.SetSpeed(playerSpeed);
        grid.hexesTravelled = 0;


    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        
        if (playersInGame == PhotonNetwork.PlayerList.Length - NetworkManager.instance.spectator.Count)
        {
            if (!NetworkManager.instance.spectator.Contains(PhotonNetwork.NickName))
                SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        //Debug.Log("[***(Players in game: " + playersInGame + ")***]");
        //Debug.Log("[***(Player list length: " + PhotonNetwork.PlayerList.Length + ")***]");
        //print("spawning player");
        int spawnPoint1;
        int spawnPoint2;
        string playerPrefabLocation;
        if (PhotonNetwork.IsMasterClient)
        {
            playerPrefabLocation = playerOnePrefabLocation;
            spawnPoint1 = 0;
            spawnPoint2 = 1;
            //clocks.GetComponent<ChessClockController>().player1Name.text = PhotonNetwork.NickName + "\nTurn";

            //Hex points is these 2 points
        }
        else
        {
            playerPrefabLocation = playerTwoPrefabLocation;
            spawnPoint1 = 2;
            spawnPoint2 = 3;
            //clocks.GetComponent<ChessClockController>().player2Name.text = PhotonNetwork.NickName + "\nTurn";

        }


        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[0].Position, Quaternion.identity);

        Transform bot1 = playerObject.transform.Find("Tank");
        Unit bot1Unit = bot1.GetComponent<Unit>();
        if (bot1Unit)
        {
            mapController.CreateUnit(spawnPoints[spawnPoint1], bot1Unit);
        }

        Transform bot2 = playerObject.transform.Find("Troop");
        Unit bot2Unit = bot2.GetComponent<Unit>();
        if (bot2Unit)
        {
            mapController.CreateUnit(spawnPoints[spawnPoint2], bot2Unit);
        }

        ////initialize the player
        PlayerController playerScript = playerObject.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        // playerScript.grid = grid;
        //assignPlayerName();
    }


    public PlayerController GetPlayer(int playerID)
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
        clocks.GetComponent<ChessClockController>().SwapClock();
        foreach (PlayerController player in players)
        {
            player.actionCount = 0;
            player.Turn = !player.Turn;
            player.setTurn(player.Turn);
            foreach (Transform child in player.gameObject.transform)
            {
                child.GetComponent<BotController>().isSelected = false;
                child.GetComponent<BotController>().attackingMode = false;
                child.GetComponent<BotController>().ResetAllMode();
            }

        }
    }
    [PunRPC]
    public void EndGame()
    {
        foreach (PlayerController player in players)
        {
            player.endGame = true;
        }
    }

    public void assignPlayerName()
    {
        foreach (PlayerController player in players)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                clocks.GetComponent<ChessClockController>().player1Name.text = PhotonNetwork.NickName + "\nTurn";
                if (player.transform.name != PhotonNetwork.NickName)
                {
                    clocks.GetComponent<ChessClockController>().player2Name.text = player.transform.name + "\nTurn";
                }

            }
            else
            {
                clocks.GetComponent<ChessClockController>().player2Name.text = PhotonNetwork.NickName + "\nTurn";
                if (player.transform.name != PhotonNetwork.NickName)
                {
                    clocks.GetComponent<ChessClockController>().player1Name.text = player.transform.name + "\nTurn";
                }
            }
        }
    }
}

