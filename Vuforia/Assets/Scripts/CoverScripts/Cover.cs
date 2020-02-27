using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CoverType
{
    FullCover,
    PartialCover,
    CoverTypeCount
}

public class Cover : MonoBehaviour
{
    
    public string ID
    {
        get
        {
            return id;
        }
    }

    public HexCell parentCell
    {
        get
        {
            return cellPosition;
        }
        set
        {
            //Add the position offsets and orientation offsets here
            this.cellPosition = value;
        }
    }

    public HexDirection direction;



    CoverType coverType;
    string id;
    HexCell cellPosition;
}
