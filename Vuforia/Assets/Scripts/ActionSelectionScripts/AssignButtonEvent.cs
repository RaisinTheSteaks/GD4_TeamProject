using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class AssignButtonEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public Button moveButton;
    public Button attackButton;
    public Button guardButton;
    public Button specialAbilitiesButton;
    public bool allAssigned;
    private int frame;

    void Start()
    {
        allAssigned = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        assignListener();
        
    }

    public void assignListener()
    {
        
        if (!allAssigned)
        { //check if all button have been assigned or not

            PlayerController player = GameManager.instance.GetPlayer(PhotonNetwork.NickName);
            if (player)
            {
                // assign all action buttons after the player exist in the game
                foreach (Transform child in player.transform)
                {
                    BotController botScript = child.GetComponent<BotController>();
                    moveButton.onClick.AddListener(delegate { botScript.move(); });
                    attackButton.onClick.AddListener(delegate { botScript.attack(); });
                    guardButton.onClick.AddListener(delegate { botScript.guard(); });
                    specialAbilitiesButton.onClick.AddListener(delegate { botScript.abilities(); });
                }

                 allAssigned = true;
            }


        }
       
    }


}
