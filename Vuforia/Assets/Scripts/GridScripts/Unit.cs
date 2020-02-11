using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Going to Start out the unit spawning according to the tutorial, then modify it
 to spawn the appropriate bots
     */

public class Unit : MonoBehaviour
{
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if(location)
            {
                location.unit = null;
            }
            location = value;
            value.unit = this;
            transform.localPosition = value.Position;
        }
    }
    HexCell location;

    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }
    float orientation;

    //Probably just set this to inactive. Needs further investigation
    public void Die()
    {
        location.unit = null;
        Destroy(gameObject);
    }
}
