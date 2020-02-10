using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanelTransition : MonoBehaviour
{

    public GameObject actionButton;
    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject guardButton;
    public GameObject abilitiesButton;

    public ActionPanel[] panels;

    // Start is called before the first frame update
    void Start()
    {
        panels = new ActionPanel[4];
        panels[0] = new ActionPanel(moveButton, moveButton.transform.position, moveButton.transform.localScale, false, 5);
        panels[1] = new ActionPanel(attackButton, attackButton.transform.position, attackButton.transform.localScale, false, 5);
        panels[2] = new ActionPanel(guardButton, guardButton.transform.position, guardButton.transform.localScale, false, 5);
        panels[3] = new ActionPanel(abilitiesButton, abilitiesButton.transform.position, abilitiesButton.transform.localScale, false, 5);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ActionPanel panel in panels)
        {
            panel.transition(actionButton.transform.position);
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
