using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class ActionPanelTransition : MonoBehaviour
{

    public GameObject actionButton;
    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject guardButton;
    public GameObject abilitiesButton;
    public GameObject moveIcon;
    public GameObject attackIcon;
    public GameObject guardIcon;
    public GameObject abilitiesIcon;
    public int speed;
    public PlayerController player;

    public ActionPanel[] panels;

    // Start is called before the first frame update
    void Start()
    {
        panels = new ActionPanel[8];
        panels[0] = new ActionPanel(moveButton, moveButton.transform.position, moveButton.transform.localScale, false, speed);
        panels[1] = new ActionPanel(attackButton, attackButton.transform.position, attackButton.transform.localScale, false, speed);
        panels[2] = new ActionPanel(guardButton, guardButton.transform.position, guardButton.transform.localScale, false, speed);
        panels[3] = new ActionPanel(abilitiesButton, abilitiesButton.transform.position, abilitiesButton.transform.localScale, false, speed);
        panels[4] = new ActionPanel(moveIcon, moveIcon.transform.position, moveIcon.transform.localScale, false, speed);
        panels[5] = new ActionPanel(attackIcon, attackIcon.transform.position, attackIcon.transform.localScale, false, speed);
        panels[6] = new ActionPanel(guardIcon, guardIcon.transform.position, guardIcon.transform.localScale, false, speed);
        panels[7] = new ActionPanel(abilitiesIcon, abilitiesIcon.transform.position, abilitiesIcon.transform.localScale, false, speed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (ActionPanel panel in panels)
        {
            panel.transition(actionButton.transform.position);
        }

        player = GameManager.instance.GetPlayer(GameObject.Find(PhotonNetwork.NickName));

        if (!player.Turn)
        {
            foreach (ActionPanel panel in panels)
            {
                panel.show = false;
                actionButton.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            actionButton.GetComponent<Button>().interactable = true;

        }
    }

    public void showHidePanel()
    {
        foreach (ActionPanel panel in panels)
        {
            panel.show = !panel.show;
        }
    }


}
