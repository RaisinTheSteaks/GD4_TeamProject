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
    public string botName;

    [Header("Info")]
    public bool isSelected = false;
    public PlayerController playerScript;
    public TextMeshProUGUI AttackTarget;

    [Header("Component")]
    public Rigidbody rig;
    public bool specialAbilityMode;
    public bool specialAbilityUsed;
    public bool guardMode;
    public HexGrid hexGrid;
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
    public bool attackingMode = false;
    private bool updatingHealth;
    private bool once;
    private const int minRng = 1, maxRng = 21;
    private const int maxRayDistance = 100;
    public Material symbol;
    public float range;
    public float gridScale;
    private GameObject attackRangeIndicator;
    public bool troopAbility;
    public bool doubleDamage = false;
    public bool showBubble;
    public ParticleSystem attackedSparks;
    public ParticleSystem muzzleEffect;
    public ParticleSystem guardEffect;
    public string tooFarResponse = "Sire, the enemy target is too far!";
    public string coverOnTheWay = "According to my calculation, there is a foreign object in the way!";
    public string alliedBotOnTheWay = "Sire, allied bot is in the way!";
    private Animator animator;

    //Pause Screen
    public bool pause;


    public void InitializeBot()
    {
        //photonPlayer = player;
        //id = player.ActorNumber;

        //if (!photonView.IsMine)
        //{
        //    rig.isKinematic = false;
        //}

    }
    private void Awake()
    {
        botPopUp = transform.parent.GetComponent<PlayerController>().popUp;
        pause = transform.parent.GetComponent<PlayerController>().pause;

    }
    private void Start()
    {
        playerScript = transform.parent.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        hexGrid = GameManager.instance.grid;
        //botPopUp.SetActive(false);
        Debug.Log(hexGrid.transform.localScale.x);
        gridScale = hexGrid.transform.localScale.x;

        maxHealth = health;
        botName = transform.name;
        transform.name = playerScript.name + " " + transform.name;
        healthNumberIndicator.text = ((int)health).ToString();
        healthBarRect = healthBar.GetComponent<RectTransform>();
        maxWidth = healthBarRect.rect.width;
        audioSource = GetComponent<AudioSource>();
        AttackTarget = GameObject.Find("AttackDebug").transform.Find("Text").GetComponent<TextMeshProUGUI>();
        
        showBubble = false;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("StartDamage", RpcTarget.All, transform.name, 15.0f, 5.0f);

        }

        AttackingPhase();
        UpdateHealth();
        SelectedText();

        DespawnAttackRange();
        Explosion(); //first part of tank Special Ability
        Heal();
        if (confirm && !specialAbilityUsed)
        {
            LoadExplosion();   //second part of tank special Ability
        }
        if (isSelected)
        {
            playerScript.botSymbol.GetComponent<RawImage>().material = symbol;
        }
        
    }
    public bool CheckActionCount()
    {
        if (transform.parent.GetComponent<PlayerController>().actionCount > 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void Move()
    {
        //debugging for action windows, replace this with real move method
        if (isSelected && playerScript.Turn && !specialAbilityMode && !pause && CheckActionCount())
        {
            ResetAllMode();
            guardMode = false;
            if (animator.GetBool("IsGuarding"))
                StartCoroutine(Animation("IsGuarding"));
            // print(transform.name + "moving");
            GameManager.instance.mapController.SetMovementState(true);
        }

    }

    public void Attack()
    {
        //debugging for action windows, replace this with real move method
        if (isSelected && playerScript.Turn && !pause && CheckActionCount())
        {

            ResetAllMode();
            //enter attacking mode
            guardMode = false;
            if (animator.GetBool("IsGuarding"))
                StartCoroutine(Animation("IsGuarding"));

            float offset = gridScale * 10.0f;


            attackingMode = !attackingMode;


            if (!attackRangeIndicator)
            {
                attackRangeIndicator = Instantiate(Resources.Load("VisualFeedback/AttackRange"), transform.position, Quaternion.identity) as GameObject;
                attackRangeIndicator.transform.localScale = new Vector3(range * offset, 0.01f, range * offset) * 0.5f;
                attackRangeIndicator.transform.SetParent(GameManager.instance.imageTarget.transform);

            }

        }

    }
    private void DespawnAttackRange()
    {
        if (!attackingMode)
        {
            if (attackRangeIndicator)
            {
                Destroy(attackRangeIndicator);
            }
        }
    }


    public void DoubleDamage()
    {
        
       doubleDamage = true;
    
    }

    public void AttackingPhase()
    {
        if (attackingMode && !pause)
        {

            //if attacking mode, pressing down mouse button will do something different
            if (Input.GetMouseButtonDown(0))
            {
               // showBubble = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, maxRayDistance))
                {
                    //casting ray
                    Debug.Log("bot detected..");
                    if (hit.transform.parent != playerScript.transform)
                    {
                        //check if the bot detected is the opponents bot
                        Debug.Log("not the same player");
                        if (hit.transform.tag == "Bot")
                        {

                            //checking if its a bot

                            Debug.Log("its a bot");
                            print(Vector3.Distance(transform.position, hit.transform.position));

                            //check if target bot is within distancce
                            if (Vector3.Distance(transform.position, hit.transform.position) < range * gridScale * 2)
                            {

                                transform.LookAt(hit.transform);
                                Vector3 offsetY = new Vector3(0, 0.001f, 0);
                                RaycastHit raycastHit;

                                //check if the ray cast hit something
                                if (Physics.Raycast(transform.position + offsetY, ((hit.transform.position + offsetY) - (transform.position + offsetY)), out raycastHit, maxRayDistance))
                                {
                                    //check if the ray cast hit a bot type game object
                                    if (raycastHit.transform.tag == "Bot")
                                    {
                                        //check if the bot is not allied
                                        if (raycastHit.transform.gameObject.GetComponent<BotController>().playerScript != playerScript)
                                        {
                                            // AttackTarget.text = "valid target";
                                            //creates random damage
                                            float rng = Random.Range(minRng, maxRng);

                                            //start shooting animation
                                            StartCoroutine(Animation("IsShooting"));

                                            //start attack audio and calculating damages

                                            photonView.RPC("AttackAudio", RpcTarget.All, transform.name);


                                            float damage = attackDamage;
                                            if (doubleDamage)
                                            {
                                                damage *= 2;
                                                doubleDamage = false;
                                            }

                                            photonView.RPC("StartDamage", RpcTarget.All, hit.transform.name, rng, damage);

                                            //set attacking moded to false
                                            attackingMode = false;

                                            //Increase player action count
                                            transform.parent.GetComponent<PlayerController>().actionCount++;

                                            //end player turn
                                            playerScript.EndTurn();
                                        }
                                        else
                                        {
                                            AttackTarget.text = alliedBotOnTheWay;
                                            showBubble = true;
                                            StartCoroutine(HideBubble());
                                        }
                                    }
                                    else
                                    {
                                        AttackTarget.text = coverOnTheWay;
                                        showBubble = true;
                                        StartCoroutine(HideBubble());
                                    }
                                }
                            }
                            else
                            {
                                AttackTarget.text = tooFarResponse;
                                showBubble = true;
                                StartCoroutine(HideBubble());
                            }

                            
                        }
                    }
                }
            }
            
        }
        
    }

    public IEnumerator HideBubble()
    {
      
            yield return new WaitForSeconds(3.0f);
            showBubble = false;
        
    }

    [PunRPC]
    public void PlayMuzzleEffect(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.muzzleEffect.Play();
    }

    [PunRPC]
    public void PlayGuardEffect(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.guardEffect.Play();
    }

    [PunRPC]
    public void StopGuardEffect(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.guardEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public IEnumerator Animation(string boolName)
    {
        
        float waitTime = 1.12f;
        //if(Type == "Tank")
        //{
        //    animator = transform.Find("Body").GetComponent<Animator>();
        //}

        animator.SetBool(boolName, true);

        //play muzzle effect if isShooting
        if(boolName == "IsShooting")
        {
            waitTime = 0.32f;
            yield return new WaitForSeconds(0.8f);
            photonView.RPC("PlayMuzzleEffect", RpcTarget.All, transform.name);
        }else if(boolName == "IsGuarding")
        {
            if (guardMode)
            {
                photonView.RPC("PlayGuardEffect", RpcTarget.All, transform.name);
                yield break;
            }
            else
            {
                photonView.RPC("StopGuardEffect", RpcTarget.All, transform.name);
                waitTime = 0;
            }
        }

        yield return new WaitForSeconds(waitTime);
        animator.SetBool(boolName, false);


    }

    public void Guard()
    {
        //debugging for action windows, replace this with real move method

        if (isSelected && playerScript.Turn && !specialAbilityMode &&  !guardMode && CheckActionCount())
        {
            Debug.Log(transform.name + "guarding");

            photonView.RPC("GuardPhase", RpcTarget.All, transform.name);
            //start shooting animation
            StartCoroutine(Animation("IsGuarding"));

            //guardPhase(transform.name);
            //end player turn
            // playerScript.EndTurn();
        }

    }

    [PunRPC]
    public void GuardPhase(string botName)
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
    public void StartDamage(string botName, float bonusDamage, float normalDamage)
    {
        StartCoroutine(Damage(botName, bonusDamage, normalDamage));
    }

    public IEnumerator Damage(string botName, float bonusDamage, float normalDamage)
    {
        yield return new WaitForSeconds(1.12f);
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();

        target.updatingHealth = true;

  
        if (normalDamage > 0)
            target.attackedSparks.Play();

        //print(target.health);
        //Half damage taken if player has entered guard
        if (target.guardMode && normalDamage > 0)
        {
            target.health -= (bonusDamage + normalDamage) / 2;
            target.guardMode = false;
            StartCoroutine(target.Animation("IsGuarding"));
        }
        else
        {
            target.health -= bonusDamage + normalDamage;
            if (target.health > target.maxHealth)
                target.health = target.maxHealth;
        }

        
    }

    [PunRPC]
    public void AttackAudio(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.audioSource.PlayOneShot(target.attackSound);
    }

    [PunRPC]
    public void DeathAudio(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.audioSource.PlayOneShot(target.deathSound);
    }


    public void UpdateHealth()
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
                StartCoroutine(DestroyBot());

            }




            updatingHealth = false;
        }
    }

    public IEnumerator DestroyBot()
    {
        photonView.RPC("DeathAudio", RpcTarget.All, transform.name);
        yield return new WaitForSeconds(0.3f);
        playerScript.CheckChildren();
        transform.gameObject.SetActive(false);
    }


    public void Abilities()
    {
        Debug.Log("Activating: " + name + "'s ability\nIsSelected: " + isSelected + ", Turn?: " + playerScript.Turn + "\nSpecAbil Used?: " + ", Paused?: " + pause);
        //debugging for action windows, replace this with real move method
        if (isSelected && playerScript.Turn && !specialAbilityUsed && !pause && CheckActionCount())
        {
            ResetAllMode();
            guardMode = false;
            if (animator.GetBool("IsGuarding"))
                StartCoroutine(Animation("IsGuarding"));
            if (Type.Equals("Tank"))
                specialAbilityMode = true;
            else
                troopAbility = true;
        }

    }

    private void Heal()
    {
        if (troopAbility && !specialAbilityUsed && !pause)
        {
            if (Input.GetMouseButtonDown(0)) //this if statement creates a raycast that checks if the player has touched a hexagon.
            {
                print("troop ability");
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
             
                if (Physics.Raycast(ray, out hit, maxRayDistance))
                {
                   if (hit.transform.tag == "Bot") 
                   {
                        if (hit.transform.parent == playerScript.transform)
                            photonView.RPC("StartDamage", RpcTarget.All, hit.transform.name, 0.0f, -30.0f);
                        specialAbilityUsed = true;
                        playerScript.EndTurn();
                    }
                }
            }
        }
    }

    private void Explosion()
    {
        if (specialAbilityMode && Type.Equals("Tank") && !pause) //checks if the player has hit the special ability button and that they haven't used this bots special ability before
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

    private void LoadExplosion()
    {
        HexCell hex = hexGrid.GetCell(tap);             //if the player has confirmed the area they want to attack then a hex is created with the tap location.
                                                        //Sphere is for debugging purposes                                  
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = hex.transform.position;
        sphere.transform.localScale = new Vector3(0.035f, 0.02f, 0.035f);
        hitColliders = Physics.OverlapSphere(hex.transform.position, 0.035f);
        photonView.RPC("MissileAudio", RpcTarget.All, transform.name);
        for (int i = 0; i < hitColliders.Length; i++)                               //are then placed in an array called hitColliders. A for loop then iterates through the hitColliders arrayand if the object 
        {
            if (hitColliders[i].transform.parent != playerScript.transform)
            {                                                                       // is a Bot then the "Start Damage function is called." Once the loop is completed the "specialAbilityUsed" boolean is turned true
                if (hitColliders[i].transform.tag == "Bot")                         //stopping this bot from using their special ability again.
                {
                    photonView.RPC("StartDamage", RpcTarget.All, hitColliders[i].transform.name, 30.0f, 0.0f);
                }
            }
        }

        StartCoroutine(DespawnSphere(sphere));
        specialAbilityUsed = true;
        transform.parent.GetComponent<PlayerController>().actionCount = 2;

    }

    public IEnumerator DespawnSphere(GameObject sphere)
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(sphere);
    }

    [PunRPC]
    public void MissileAudio(string botName)
    {
        GameObject bot = GameObject.Find(botName);
        BotController target = bot.GetComponent<BotController>();
        target.audioSource.PlayOneShot(target.specialAbilitySound);
    }

    public void ResetAllMode()
    {
        attackingMode = false;
        specialAbilityMode = false;
        //GameManager.instance.mapController.SetMovementState(false);

    }

    private void SelectedText()
    {
        if (isSelected)
        {
            if (once)
            {
                StartCoroutine(Animation("IsSelected"));
                once = false;
            }


        }
        else if (isSelected == false)
        {
            once = true;
            ResetAllMode();
        }


    }

}