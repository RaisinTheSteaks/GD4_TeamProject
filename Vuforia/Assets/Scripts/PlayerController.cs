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

    [Header("Info")]
    public string coalition;
    public bool selected = false;

    [Header("Component")]
    public Rigidbody rig;
    public Player photonPlayer;
    public TextMeshProUGUI playerNickname;
    private Transform canvas;
    private Transform arrow;
    public Animator animator;
   



    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        coalition = player.NickName;
        

        GameManager.instance.players[id - 1] = this;

        if(!photonView.IsMine)
        {
            rig.isKinematic = true;
        }
        
    }

    private void Start()
    {
        playerNickname.text = photonPlayer.NickName;
        canvas = transform.Find("Canvas");
        arrow = canvas.Find("Arrow");
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        arrow.gameObject.SetActive(selected);
        if(selected == true)
        {
            GetComponent<Animator>().SetBool("IsShooting", false);
        }
    }

    public void shoot()
    {
        //if(GetComponent<Animator>() != null && GetComponent<Animator>().isActiveAndEnabled)
        // {
         
        if(selected)
        {
            selected = false;
            print(coalition + ": " + transform.name + " shoot");
            GetComponent<Animator>().SetBool("IsShooting", true);
            GameManager.instance.photonView.RPC("DestroyTarget", RpcTarget.All, GameManager.instance.selectedTarget.transform.name);
        }
        
      //  }
        
    }

    public void initiateRespawnTarget()
    {
        GameManager.instance.photonView.RPC("RespawnAllTarget", RpcTarget.All);
    }

}
