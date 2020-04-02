using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineManager : MonoBehaviour
{
    public VoiceLineClass[] voiceLines;

    string action;
    string botName;

    public void SetBotName(string s)
    {
        Debug.Log("Setting Bot Name: " + s);
        botName = s;
    }
    public void SetAction(string a)
    {
        Debug.Log("Setting action: " + a);
        action = a;
    }

    public void PlayVoiceLine()
    {
        string debug = "ERROR Not able to find line: [" + botName + ", " + action + "]";
        AudioSource audio = new AudioSource();
        for (int i = 0; i < voiceLines.Length; i++)
        {
            Debug.Log("Checking Voice line: [" + voiceLines[i].botName + ", " + voiceLines[i].action + "]");

            if (voiceLines[i].botName == botName)
            {
                if (voiceLines[i].action == action)
                {
                    debug = "Found Voice line: [" + botName + ", " + action + "]";
                    audio = voiceLines[i].audioSource;
                    goto FOUND;
                }
            }
        }
        FOUND:;
        Debug.Log(debug);

        if (audio.clip != null)
        {
            audio.PlayOneShot(audio.clip);
        }
    }
}
