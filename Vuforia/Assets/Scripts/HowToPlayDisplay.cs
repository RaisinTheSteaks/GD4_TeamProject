using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class HowToPlayDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] videos;
    public GameObject[] beads;
    public int currentVideo;
    void Start()
    {
        currentVideo = 0;
        SetVideoActive(currentVideo);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnNextButtonClick()
    {
        if((currentVideo + 1) != videos.Length)
        {
            currentVideo++;
            SetVideoActive(currentVideo);
        }
        
    }

    public void OnBackButtonClick()
    {
        if(currentVideo - 1 != -1)
        {
            currentVideo--;
            SetVideoActive(currentVideo);
        }
        
    }

    public void SetVideoActive(int index)
    {
        foreach(GameObject video in videos)
        {
            video.transform.Find("VideoPlayer").GetComponent<VideoPlayer>().Stop();
            video.SetActive(false);
        }
        foreach(GameObject bead in beads)
        {
            bead.GetComponent<Image>().color = Color.white;
        }

        videos[index].SetActive(true);
        videos[index].transform.Find("VideoPlayer").GetComponent<VideoPlayer>().Play();

        beads[index].GetComponent<Image>().color = new Color(250/255.0f, 232.0f/255.0f, 22.0f/255.0f);
    }

    public void ResetVideo()
    {
        foreach (GameObject video in videos)
        {
            video.transform.Find("VideoPlayer").GetComponent<VideoPlayer>().Stop();
            currentVideo = 0;
        }
    }


}
