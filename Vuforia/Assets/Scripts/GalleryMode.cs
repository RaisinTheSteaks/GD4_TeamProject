using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
public enum GalleryBot
{
    BlueTroop = 0,
    RedTroop,
    BlueTank,
    RedTank,
    Count
}

public class GalleryMode : MonoBehaviourPunCallbacks
{
    public string menuScene;
    public GameObject blueTroop;
    public GameObject redTroop;
    public GameObject blueTank;
    public GameObject redTank;
    public Scrollbar scaler;
    private const float originalScale = 0.5f;

    private void Start()
    {
        blueTroop.SetActive(true);
        redTroop.SetActive(true);
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
        blueTroop.transform.localScale = Vector3.one * originalScale * modifier;
        redTroop.transform.localScale = Vector3.one * originalScale * modifier;
        blueTank.transform.localScale = Vector3.one * originalScale * modifier;
        redTank.transform.localScale = Vector3.one * originalScale * modifier;
    }

    public void MakeBotActive(int bot)
    {
        switch((GalleryBot)bot)
        {
            case GalleryBot.BlueTroop:
                HideAllBot();
                blueTroop.SetActive(true);
                break;

            case GalleryBot.RedTroop:
                HideAllBot();
                redTroop.SetActive(true);
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
        blueTroop.SetActive(false);
        redTroop.SetActive(false);
        blueTank.SetActive(false);
        redTank.SetActive(false);
    }

    public void OnBackToMenu()
    {
        NetworkManager.instance.ChangeScene(menuScene);
    }

   
}
