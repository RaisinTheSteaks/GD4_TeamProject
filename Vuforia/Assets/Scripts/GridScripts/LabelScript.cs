using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LabelScript : MonoBehaviour
{
    Sprite sprite;
    float i = 0f;
    void Awake()
    {
        sprite = GetComponentInChildren<Sprite>();    
    }

    // Update is called once per frame
    void Update()
    {
        i++;
        sprite.textureRect.Set(0f,0f,i,i);
        if(i>15)
        {
            i = 0;
        }
    }
}
