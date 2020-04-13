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
            int multiplier = 1;
            if(transform.rotation.y < 0)
            {
                multiplier = -1;
            }
            transform.localPosition = value.Position +  new Vector3(0,0,8f) * multiplier;
            //transform.position = transform.localPosition;
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
