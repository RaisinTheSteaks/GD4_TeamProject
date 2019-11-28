using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " has entered trigger volume: " + this.name);
        other.transform.TransformPoint(this.transform.position.x,0.0f,this.transform.position.z);
    }
}
