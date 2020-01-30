using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudioManager : MonoBehaviour
{
    [Header("VoiceLines")]
    public VoiceLine line;
    public VoiceLine[] voiceLines=new VoiceLine[1];
    public AudioSource clip;
    public int x;

    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(0.01f); 
        clip.Play();    
    }


    public void PlayVoiceLine(VoiceLineCharacter character, VoiceLineType type, string name)
    {
        foreach (VoiceLine line in voiceLines)
        {
            if (line.name == name)
            {
                if(line.type==type)
                {
                    if(line.character==character)
                    {
                        AudioSource audio = GetComponent<AudioSource>();
                        audio.clip = line.audioClip;
                        audio.Play();
                    }
                }
            }
        }
    }
}
