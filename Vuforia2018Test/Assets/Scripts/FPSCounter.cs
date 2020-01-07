using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsUI;
    int frameCount = 0;
    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.

    void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / updateRate)
        {
            fps = frameCount / dt;
            
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }

        fpsUI.text = "FPS: " + System.Math.Round(fps,1).ToString();

    }
}
