using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GalleryBot
{
    Troop = 0,
    BlueTank,
    RedTank,
    Count
}

public class GalleryMode : MonoBehaviour
{
    public string menuScene;
    public GameObject troop;
    public GameObject blueTank;
    public GameObject redTank;
    public Scrollbar scaler;
    private const float originalScale = 0.5f;

    private void Start()
    {
        troop.SetActive(true);
        blueTank.SetActive(false);
        redTank.SetActive(false);
 
    }

   
    public void ScaleSize()
    {
        SetScale(scaler.value);
    }

    public void SetScale(float value)
    {
        if(value <= 0.1)
        {
            value = 0.1f;
        }
        float modifier = value / 0.5f;
        troop.transform.localScale = Vector3.one * originalScale * modifier;
        blueTank.transform.localScale = Vector3.one * originalScale * modifier;
        redTank.transform.localScale = Vector3.one * originalScale * modifier;
    }

    public void MakeBotActive(int bot)
    {
        switch((GalleryBot)bot)
        {
            case GalleryBot.Troop:
                HideAllBot();
                troop.SetActive(true);
                break;

            case GalleryBot.BlueTank:
                HideAllBot();
                blueTank.SetActive(true);
                break;

            case GalleryBot.RedTank:
                HideAllBot();
                redTank.SetActive(true);
                break;

        }
    }

    public void HideAllBot()
    {
        troop.SetActive(false);
        blueTank.SetActive(false);
        redTank.SetActive(false);
    }

    public void OnBackToMenu()
    {
        SceneManager.LoadScene(menuScene, LoadSceneMode.Single);
    }
}
