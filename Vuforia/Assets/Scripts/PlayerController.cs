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

    [PunRPC]
    public void Initialize(Player player)
    {
        transform.SetParent(GameManager.instance.imageTarget.transform);
        photonPlayer = player;
        id = player.ActorNumber;

       foreach(Transform child in transform)
        {
            BotController botScript = child.GetComponent<BotController>();
            botScript.InitializeBot();
        }

    }

    private void Start()
    {

    }

    private void Update()
    {

    }

   



}
