using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

public class BotController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public bool isSelected = false;
    public PlayerController playerScript;

    [Header("Component")]
    public Rigidbody rig;
    
    public TextMeshProUGUI SelectedStatus;

 
    public void InitializeBot()
    {
        //photonPlayer = player;
        //id = player.ActorNumber;

        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }

    }

    private void Start()
    {
        playerScript = transform.parent.GetComponent<PlayerController>();
    }

    private void Update()
    {
        //debuging purposes, will delete later
        if (playerScript.Turn)
        SelectedText();
    }


    public void move()
    {
        //debugging for action windows, replace this with real move method
        if(isSelected && playerScript.Turn)
        {
            print(transform.name + "moving");
        }
        
    }

    public void attack()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.Turn)
        {
            print(transform.name + "attacking");
        }

    }

    public void guard()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.Turn)
        {
            print(transform.name + "guarding");
        }

    }

    public void abilities()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.Turn)
        {
            print(transform.name + "using abilities");
        }

    }

    private void SelectedText()
    {
        if (isSelected)
            SelectedStatus.text = "Selected";
        else if (isSelected == false)
            SelectedStatus.text = "Not Selected";
    }


}
