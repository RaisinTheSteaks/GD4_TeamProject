

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour
{

    public void YesButton()
    {
        GameObject popUp;
        popUp = GetComponent<GameObject>();
        popUp = GameObject.Find("PopUp");
        popUp.SetActive(false);

        //create a for each that goes to every bot in the gma eand sets their special ability to false.
        var objects = GameObject.FindGameObjectsWithTag("Bot");
        foreach (GameObject bot in objects)
        {
            bot.transform.GetComponent<BotController>().specialAbility = false;
            bot.transform.GetComponent<BotController>().confirm = true;
            bot.transform.GetComponent<BotController>().specialAbilityUsed = true;
        }
    }
    public void NoButton()
    {
        GameObject popUp;
        popUp = GetComponent<GameObject>();
        popUp = GameObject.Find("PopUp");
        popUp.SetActive(false);

        var objects = GameObject.FindGameObjectsWithTag("Bot");
        foreach (GameObject bot in objects)
        {
            bot.transform.GetComponent<BotController>().specialAbility = false;
            bot.transform.GetComponent<BotController>().confirm = false;
            bot.transform.GetComponent<BotController>().specialAbilityUsed = false;
        }

    }
}
