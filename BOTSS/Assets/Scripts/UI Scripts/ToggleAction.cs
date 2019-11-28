using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAction : MonoBehaviour
{
    private bool showWindow;
    public GameObject gameObject;
    
    private RectTransform windowTransform;
    private float minX;
    private float maxX;
    private Vector3 speed = new Vector3(15f,0,0);
    
    void Start()
    {
        //get the width of the window
        windowTransform = gameObject.GetComponent<RectTransform>();

        showWindow = false;

        //setting the minimum x and maximum x (set an area for a window to operate)
        minX = gameObject.transform.position.x;
        maxX = windowTransform.rect.x + windowTransform.rect.width;
        
    }

    // Update is called once per frame
    void Update()
    {
        moveWindow();
        
    }

    //toggle the window hide/unhide
    public void toggleHideWindow()
    {
        
        showWindow = !showWindow;
        
    }

    //creating transition when toggled
    public void moveWindow()
    {
        if (showWindow)
        {

            if (gameObject.transform.position.x <= maxX)
            {
                gameObject.transform.position += speed;
            }

        }
        else
        {

            if (gameObject.transform.position.x >= minX)
            {

                gameObject.transform.position -= speed;
            }
        }
    }
}
