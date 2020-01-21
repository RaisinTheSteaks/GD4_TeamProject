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
    public AudioClip attackSound;
    public AudioSource audioSource;
    public AudioClip deathSound;

    [Header("Stat")]
    public float maxHealth;
    public float health;
    public float attackDamage;
    private bool attackingMode;
    private bool updatingHealth;

    
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
        maxHealth = health;
        transform.name = playerScript.name + " " + transform.name;
        healthNumberIndicator.text = ((int)health).ToString();
        healthBarRect = healthBar.GetComponent<RectTransform>();
        maxWidth = healthBarRect.rect.width;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

        attackingPhase();
        updateHealth();
        

        //debuging purposes, will delete later
        SelectedText();
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
                            photonView.RPC("attackAudio", RpcTarget.All, transform.name);
                            photonView.RPC("startDamage", RpcTarget.All, hit.transform.name, rng);
                            
                            attackingMode = false;
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    public void startDamage(string botName, float bonusDamage)
    {
        StartCoroutine(damage(botName, bonusDamage));
    }

    public IEnumerator damage(string botName, float bonusDamage)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.health -= attackDamage + bonusDamage;
        target.updatingHealth = true;
  
        print(target.health);

    }

    [PunRPC]
    public void attackAudio(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.audioSource.PlayOneShot(target.attackSound);
    }

    [PunRPC]
    public void deathAudio(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.audioSource.PlayOneShot(target.deathSound);
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
                health = 0;
                StartCoroutine(destroyBot());

            }


            updatingHealth = false;
        }
    }

    public IEnumerator destroyBot()
    {
        photonView.RPC("deathAudio", RpcTarget.All, transform.name);
        yield return new WaitForSeconds(0.3f);
        transform.gameObject.SetActive(false);
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

    private void SelectedText()
    {
        if (isSelected)
            SelectedStatus.text = "Selected";
        else if (isSelected == false)
            SelectedStatus.text = "Not Selected";
    }


}
