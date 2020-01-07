using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ButtonEvent : MonoBehaviour
{
    public Button shootButton;
    private List<GameObject> units;
    private bool assigned = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

        units = GameManager.instance.GetAllUnits(PhotonNetwork.NickName);
      
        if(!assigned && units.Count > 0)
        {
            foreach (GameObject unit in units)
            {
                PlayerController player = unit.GetComponent<PlayerController>();
                shootButton.onClick.AddListener(delegate { player.shoot(); });
                assigned = true;
            }
        }
        

    }
}
