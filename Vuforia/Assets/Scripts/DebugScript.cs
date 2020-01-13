using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DebugScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerController player = GameManager.instance.GetPlayer(PhotonNetwork.NickName);
            if(player.isMyTurn)
            {
                BotController botScript = player.transform.GetChild(0).gameObject.GetComponent<BotController>();
                botScript.isSelected = !botScript.isSelected;
                if (botScript.isSelected)
                {
                    print("selected bot 1");
                }
                else
                {
                    print("deselected bot 1");
                }
            }
            
        }else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            foreach(PlayerController player in GameManager.instance.players)
            {
                player.isMyTurn = !player.isMyTurn;
                if(player.isMyTurn)
                {
                    print(player.transform.name + " turn");
                }
            }
        }
    }
}
