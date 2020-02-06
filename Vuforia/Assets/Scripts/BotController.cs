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
    //variables created & used for the TankSpecial Ability
    Vector3 tap = new Vector3();
    Ray ray;
    public bool confirm = false;

    [Header("Info")]
    public bool isSelected = false;
    public PlayerController playerScript;
    public TextMeshProUGUI SelectedStatus;

    [Header("Component")]
    public Rigidbody rig;
    public bool specialAbility = false;
    public bool specialAbilityUsed;
    public GameObject hexGrid;

    Collider[] hitColliders;
    int health = 100;

    [Header("PopUp")]
    public GameObject popUp;


    public void InitializeBot()
    {
        //photonPlayer = player;
        //id = player.ActorNumber;

        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
            specialAbility = false;
        }

    }

    private void Start()
    {
        playerScript = transform.parent.GetComponent<PlayerController>();
        popUp = GetComponent<GameObject>();
        hexGrid = GetComponent<GameObject>();
        popUp = GameObject.Find("PopUp");
        hexGrid = GameObject.Find("HexGrid");
        popUp.SetActive(false);
    }
    private void Update()
    {
        //debuging purposes, will delete later
        if (playerScript.Turn && SelectedStatus.text != "BANG BANG")
            SelectedText();
        if (specialAbility && !specialAbilityUsed)
            ExplosionDamage();
        if (!popUp.activeSelf && confirm)
            loadExplosion();
    }


    public void move()
    {
        //debugging for action windows, replace this with real move method
        if(isSelected && playerScript.Turn && !specialAbility)
        {
            print(transform.name + "moving");
        }
        
    }

    public void attack()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.Turn && !specialAbility)
        {
            print(transform.name + "attacking");
        }

    }

    public void guard()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.Turn && !specialAbility)
        {
            print(transform.name + "guarding");
        }

    }

    public void abilities()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.Turn)
        {
            specialAbility = true;
        }

    }

    private void loadExplosion()
    {
        HexCell hex = hexGrid.GetComponent<HexGrid>().getCell(tap);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = hex.transform.position;
        sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        hitColliders = Physics.OverlapSphere(hex.transform.position, 0.5f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].transform.name == transform.name)
            {
                hitColliders[i].transform.GetComponent<BotController>().SelectedStatus.text = "BANG BANG";
            }
        }
        specialAbilityUsed = true;
    }

    void ExplosionDamage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(!popUp.activeSelf && !confirm)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    tap = hit.point;
                    popUp.SetActive(true);
                }
                //this will be where the damage is dealt
            }
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
