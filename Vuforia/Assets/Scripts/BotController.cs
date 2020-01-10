using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;



public class BotController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public bool isSelected = false;
    public PlayerController playerScript;

    [Header("Component")]
    public Rigidbody rig;


    [Header("Stat")]
    public float health;
    public float attackDamage;

    private bool attackingMode;
 
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
        attackingPhase();
        
    }

    public void move()
    {
        //debugging for action windows, replace this with real move method
        if(isSelected && playerScript.isMyTurn)
        {
            print(transform.name + "moving");
        }
        
    }

    public void attack()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected)
        {
            attackingMode = true;
            print("attacking...");
        }

    }

    public void attackingPhase()
    {
        if(attackingMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    print("bot detected..");
                    if(hit.transform.parent != playerScript.transform)
                    {
                        print("not the same player");
                        if(hit.transform.tag == "Bot")
                        {
                            print("its a bot");
                            BotController target = hit.transform.gameObject.GetComponent<BotController>();
                            damage(target);
                            attackingMode = false;
                        }
                    }
                }
            }
        }
    }

    public void damage(BotController target)
    {

        float rng = Random.Range(1, 21);
        target.health -= attackDamage + rng;
        print(target.health);
    }

    public void guard()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.isMyTurn)
        {
            print(transform.name + "guarding");
        }

    }

    public void abilities()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.isMyTurn)
        {
            print(transform.name + "using abilities");
        }

    }



}
