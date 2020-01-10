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
    public bool isMyTurn = false;

    [PunRPC]
    public void Initialize(Player player)
    {
        transform.SetParent(GameManager.instance.imageTarget.transform);
        photonPlayer = player;
        id = player.ActorNumber;

       GameManager.instance.players[id - 1] = this;

       foreach(Transform child in transform)
        {
            BotController botScript = child.GetComponent<BotController>();
            botScript.InitializeBot();
        }

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
        
        
    }

    private void Update()
    {
        
    }

   



}
