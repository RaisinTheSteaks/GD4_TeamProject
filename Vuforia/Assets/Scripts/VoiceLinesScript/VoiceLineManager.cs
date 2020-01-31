using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineManager : MonoBehaviour
{
    public Dictionary<VoiceLine, AudioSource> lines;



    public AudioSource PickVoiceLine(string botName, string action)
    {
        string debug = "ERROR Not able to find line in map: [" + botName + ", " + action + "]";
        
        VoiceLine vl;
        vl.botName = botName;
        vl.action = action;
        AudioSource audio = new AudioSource();

        if(lines.TryGetValue(vl, out audio))
        {
            debug = "Found Voice Line: [" + botName + ", " + action + "]";
        }

        Debug.Log(debug);

        return audio;
    }
}
