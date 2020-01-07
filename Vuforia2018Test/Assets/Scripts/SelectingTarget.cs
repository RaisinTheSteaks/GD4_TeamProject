using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingTarget : MonoBehaviour
{

    public Material target;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100))
            {
                foreach(Transform child in transform)
                {
                    if(hit.transform.name == child.transform.name)
                    {
                        if(GameManager.instance.selectedTarget != child.gameObject)
                        {
                            ClearSelection();
                            child.GetComponent<Renderer>().material.color = Color.red;
                            GameManager.instance.selectedTarget = child.gameObject;
                            
                        }
                        else
                        {
                            ClearSelection();
                        }
                    }
                }
            }
        }
    }

    public void ClearSelection()
    {
        if(GameManager.instance.selectedTarget)
        {
            GameManager.instance.selectedTarget.GetComponent<Renderer>().material = target;
            
            GameManager.instance.selectedTarget = null;
        }
    }
}
