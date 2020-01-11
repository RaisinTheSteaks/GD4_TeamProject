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
    public TextMeshProUGUI healthNumberIndicator;
    public GameObject healthBar;
    private RectTransform healthBarRect;
    public float maxWidth;


    [Header("Stat")]
    public float maxHealth;
    public float health;
    public float attackDamage;
    private bool attackingMode;
    private bool updatingHealth;
 
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
        maxHealth = health;
        transform.name = playerScript.name + " " + transform.name;
        healthNumberIndicator.text = ((int)health).ToString();
        healthBarRect = healthBar.GetComponent<RectTransform>();
        maxWidth = healthBarRect.rect.width;
    }

    private void Update()
    {
        attackingPhase();
        updateHealth();
        
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
                            float rng = Random.Range(1, 21);
                            photonView.RPC("damage", RpcTarget.All, hit.transform.name, rng);
                            attackingMode = false;
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    public void damage(string botName, float bonusDamage)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        
        target.health -= attackDamage + bonusDamage;
        target.updatingHealth = true;
        print(target.health);
    }

    public void updateHealth()
    {
        if(updatingHealth)
        {
            
            healthNumberIndicator.text = ((int)health).ToString();
            healthBarRect.sizeDelta = new Vector2(maxWidth * (health / maxHealth), healthBarRect.rect.height);
            if(health/maxHealth <= 0.2)
            {
                healthBar.GetComponent<Image>().color = Color.red;
            }else if(health/maxHealth <= 0.5)
            {
                healthBar.GetComponent<Image>().color = new Color(1, 0.5f, 0);
            }

            if (health <= 0)
            {
                transform.gameObject.SetActive(false);
            }


            updatingHealth = false;
        }
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
