using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    public GameObject buttonPanel;
    public Vector2 finalPosition;
    public Vector3 finalSize;
    public bool show;
    public int speed;

    public ActionPanel(GameObject buttonPanel, Vector3 finalPosition, Vector3 finalSize, bool show, int speed)
    {
        this.buttonPanel = buttonPanel;
        this.finalPosition = finalPosition;
        this.finalSize = finalSize;
        this.show = show;
        this.speed = speed;
    }



    public void transition(Vector3 actionButton)
    {
        if(show)
        {
            //buttonPanel.SetActive(true);
            showPanel();
        }
        else
        {
            hidePanel(actionButton);
            //buttonPanel.SetActive(false);
        }

        if(buttonPanel.transform.localScale.x <= 0 && buttonPanel.transform.localScale.y <= 0)
        {
            buttonPanel.SetActive(false);
        }
        else
        {
            buttonPanel.SetActive(true);
        }

    }

    public void showPanel()
    {
        if(buttonPanel.transform.position.x <= finalPosition.x)
        {
            buttonPanel.transform.position += new Vector3(1,0,0) * speed;
        }

        if(buttonPanel.transform.position.y <= finalPosition.y)
        {
            buttonPanel.transform.position += new Vector3(0, 1, 0) * speed;
        }

        if (buttonPanel.transform.position.y >= finalPosition.y)
        {
            buttonPanel.transform.position -= new Vector3(0, 1, 0) * speed;
        }

        if (buttonPanel.transform.localScale.x <= finalSize.x)
        {
            buttonPanel.transform.localScale += new Vector3(1, 0, 0) * speed * 0.01f;
        }

        if (buttonPanel.transform.localScale.y <= finalSize.y)
        {
            buttonPanel.transform.localScale += new Vector3(0, 1, 0) * speed * 0.01f;
        }
    }

    public void hidePanel(Vector3 actionButton)
    {
        if (buttonPanel.transform.position.x >= actionButton.x)
        {
            buttonPanel.transform.position -= new Vector3(1, 0, 0) * speed;
        }

        if (buttonPanel.transform.position.y >= actionButton.y)
        {
            buttonPanel.transform.position -= new Vector3(0, 1, 0) * speed;
        }

        if (buttonPanel.transform.position.y <= actionButton.y)
        {
            buttonPanel.transform.position += new Vector3(0, 1, 0) * speed;
        }

        if (buttonPanel.transform.localScale.x >= 0)
        {
            buttonPanel.transform.localScale -= new Vector3(1, 0, 0) * speed * 0.005f;
        }

        if (buttonPanel.transform.localScale.y >= 0)
        {
            buttonPanel.transform.localScale -= new Vector3(0, 1, 0) * speed * 0.005f;
        }
    }


}
