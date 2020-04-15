

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
            if (bot.transform.GetComponent<BotController>().specialAbilityMode == true && bot.transform.GetComponent<BotController>().Type.Equals("Tank"))
            {
                bot.transform.GetComponent<BotController>().confirm = true;
                bot.transform.GetComponent<BotController>().specialAbilityMode = false;
            }
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
            bot.transform.GetComponent<BotController>().specialAbilityMode = false;
            bot.transform.GetComponent<BotController>().confirm = false;
            bot.transform.GetComponent<BotController>().crossHair.enabled = false;

        }

    }
}