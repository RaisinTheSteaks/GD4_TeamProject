using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CharacterSelection : MonoBehaviour
{
    //public Image selectedIndicator;
    private List<GameObject> units;

    private void Awake()
    {
      
    }
    void Update()
    {
        units = GameManager.instance.GetAllUnits(PhotonNetwork.NickName);
        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if(hit.transform.tag == "Player")
                {
                    
                    if(units.Contains(hit.transform.gameObject))
                    {
                        
                        PlayerController playerScript = hit.transform.gameObject.GetComponent<PlayerController>();
                        Transform canvas = hit.transform.Find("Canvas");
                        Transform arrow = canvas.transform.Find("Arrow");

                        if (playerScript.selected)
                            playerScript.selected = false;
                        else
                        {
                            UnitClearSelection();
                            playerScript.selected = true;
                        }

                        
                    }
                }
            }
        }
    }

    

    void UnitClearSelection()
    {
        foreach(GameObject unit in units)
        {
            PlayerController player = unit.gameObject.GetComponent<PlayerController>();
            player.selected = false;
        }
    }
}
