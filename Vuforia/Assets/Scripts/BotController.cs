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
    //variables created & used for the TankSpecial Ability
    Vector3 tap = new Vector3();
    Ray ray;
    public bool confirm;

    [Header("Info")]
    public bool isSelected = false;
    public PlayerController playerScript;
    public TextMeshProUGUI SelectedStatus;

    [Header("Component")]
    public Rigidbody rig;
    public bool specialAbilityMode;
    public bool specialAbilityUsed;
    public bool guardMode;
    public GameObject hexGrid;
    public string Type;
    Collider[] hitColliders;

    [Header("PopUp")]
    public GameObject botPopUp;



    public TextMeshProUGUI healthNumberIndicator;
    public GameObject healthBar;
    private RectTransform healthBarRect;
    public float maxWidth;
    public AudioClip attackSound;
    public AudioClip specialAbilitySound;
    public AudioSource audioSource;
    public AudioClip deathSound;

    [Header("Stat")]
    public float maxHealth;
    public float health;
    public float attackDamage;
    public bool attackingMode;
    private bool updatingHealth;
    private bool once;
    public Material symbol;


    public void InitializeBot()
    {
        //photonPlayer = player;
        //id = player.ActorNumber;

        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }

    }
    private void Awake()
    {
        botPopUp = transform.parent.GetComponent<PlayerController>().popUp;

    }
    private void Start()
    {
        playerScript = transform.parent.GetComponent<PlayerController>();

        hexGrid = GetComponent<GameObject>();
        hexGrid = GameObject.Find("HexGrid");
        //botPopUp.SetActive(false);


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
        SelectedText();
        //guardPhase();
        Explosion(); //first part of tank Special Ability
        if (confirm && !specialAbilityUsed)
            loadExplosion();   //second part of tank special Ability
    }


    public void Move()
    {
        //debugging for action windows, replace this with real move method
        if (isSelected && playerScript.Turn && !specialAbilityMode)
        {
           // print(transform.name + "moving");
            GameManager.instance.mapController.SetMovementState(true);
        }

    }

    public void Attack()
    {
        //debugging for action windows, replace this with real move method
        if (isSelected && playerScript.Turn)
        {
            //enter attacking mode
            attackingMode = true;
            guardMode = false;
            print("attacking...");
        }

    }

    public void attackingPhase()
    {
        if (attackingMode)
        {
            //if attacking mode, pressing down mouse button will do something different
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    //casting ray
                    print("bot detected..");
                    if (hit.transform.parent != playerScript.transform)
                    {
                        //check if the bot detected is the opponents bot
                        print("not the same player");
                        if (hit.transform.tag == "Bot")
                        {
                            //checking if its a bot
                            print("its a bot");

                            //creates random damage
                            float rng = Random.Range(1, 21);

                            //start shooting animation
                            StartCoroutine(animation("IsShooting"));

                            //start attack audio and calculating damages
                            photonView.RPC("attackAudio", RpcTarget.All, transform.name);
                            photonView.RPC("startDamage", RpcTarget.All, hit.transform.name, rng, attackDamage);
                            

                            //set attacking moded to false
                            attackingMode = false;

                            //end player turn
                            playerScript.EndTurn();
                        }
                    }
                }
            }
        }
        if (guardMode)
        {
            photonView.RPC("guardPhase", RpcTarget.All, transform.name);
        }
    }

    public IEnumerator animation(string boolName)
    {
        GetComponent<Animator>().SetBool(boolName, true);
        yield return new WaitForSeconds(1.12f);
        GetComponent<Animator>().SetBool(boolName, false);


    }
    [PunRPC]
    public void guard()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.Turn && !specialAbilityMode)
        {
            Debug.Log(transform.name + "guarding");
            guardPhase(transform.name);
            //end player turn
           // playerScript.EndTurn();
        }

    }

    [PunRPC]
    public void guardPhase(string botName)
    {
        //if (guardMode)
        //{
        //    //set attacking mode to false
        //    attackingMode = false;
        //    //start guarding animation
        //   // StartCoroutine(animation("IsGuarding"));
        //}
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.guardMode = true;
    }

    [PunRPC]
    public void startDamage(string botName, float bonusDamage, float normalDamage)
    {
        StartCoroutine(damage(botName, bonusDamage, normalDamage));
    }

    public IEnumerator damage(string botName, float bonusDamage, float normalDamage)
    {
        yield return new WaitForSeconds(0.5f);
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        
        target.updatingHealth = true;

        //print(target.health);

        //Half damage taken if player has entered guard
        if (target.guardMode)
        {
            target.health -= (bonusDamage + normalDamage) / 2;
        }
        else
        {
            target.health -= bonusDamage + normalDamage;
        }
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
        if (updatingHealth)

        {
            healthNumberIndicator.text = ((int)health).ToString();
            healthBarRect.sizeDelta = new Vector2(maxWidth * (health / maxHealth), healthBarRect.rect.height);
            if (health / maxHealth <= 0.2)
            {
                healthBar.GetComponent<Image>().color = Color.red;
            }
            else if (health / maxHealth <= 0.5)
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


    public void abilities()
    {
        //debugging for action windows, replace this with real move method
        if (isSelected && playerScript.Turn && !specialAbilityUsed)
        {
            specialAbilityMode = true;
        }

    }

    void Explosion()
    {
        if (specialAbilityMode && Type.Equals("Tank")) //checks if the player has hit the special ability button and that they haven't used this bots special ability before
        {
            if (Input.GetMouseButtonDown(0)) //this if statement creates a raycast that checks if the player has touched a hexagon.
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (!botPopUp.activeSelf && !confirm)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.name == "hexagon") //if the player has touched a hexagon
                        {
                            tap = hit.point;                //the global vector tap stores the location the player touched. 
                            botPopUp.SetActive(true);          //the popup activates.
                        }                                   //once the popup activates the popupControllers asks the player to confirm if the area they want to attack is the area they've selected.
                    }
                }
            }
        }
    }

    private void loadExplosion()
    {
        HexCell hex = hexGrid.GetComponent<HexGrid>().getCell(tap);             //if the player has confirmed the area they want to attack then a hex is created with the tap location.
                                                                                //Sphere is for debugging purposes                                  
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = hex.transform.position;
        sphere.transform.localScale = new Vector3(0.15f, 0.1f, 0.15f);
        hitColliders = Physics.OverlapSphere(hex.transform.position, 0.15f);     // An overlap sphere is then spawned at the center of hex. All objects that are touching or within the overlap sphere 
        photonView.RPC("missileAudio", RpcTarget.All, transform.name);
        for (int i = 0; i < hitColliders.Length; i++)                               //are then placed in an array called hitColliders. A for loop then iterates through the hitColliders arrayand if the object 
        {
            if (hitColliders[i].transform.parent != playerScript.transform)
            {                                                                       // is a Bot then the "Start Damage function is called." Once the loop is completed the "specialAbilityUsed" boolean is turned true
                if (hitColliders[i].transform.tag == "Bot")                         //stopping this bot from using their special ability again.
                {
                    photonView.RPC("startDamage", RpcTarget.All, hitColliders[i].transform.name, 30.0f, 0.0f);
                }
            }
        }

        StartCoroutine(despawnSphere(sphere));
        specialAbilityUsed = true;
        playerScript.EndTurn();

    }

    public IEnumerator despawnSphere(GameObject sphere)
    {
        yield return new WaitForSeconds(1.0f);
        sphere.SetActive(false);
    }

    [PunRPC]
    public void missileAudio(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.audioSource.PlayOneShot(target.specialAbilitySound);
    }


    private void SelectedText()
    {
        if (isSelected)
        {
            SelectedStatus.text = "Selected";
            if (once)
            {
                StartCoroutine(animation("IsSelected"));
                once = false;
            }

        }
        else if (isSelected == false)
        {
            SelectedStatus.text = "Not Selected";
            once = true;
        }


    }

}