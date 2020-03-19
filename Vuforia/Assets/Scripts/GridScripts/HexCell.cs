using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HexCell : MonoBehaviour
{
    #region Variables
    //The current grid-position of this cell
    public HexCoordinates coordinates;
    //The Color this cell should be shaded
    public Color color;
    
    //The debug information on top of the cell, like the highlights and the turns - to - travel information
    public RectTransform uiRect;

    //Track all of the cells next to this one
    [SerializeField]
    HexCell[] neighbors;

    //Used to set if this cell is within movement range
    public bool inRange = false;

    //Used to track the path that the player travels along
    public HexCell PathFrom { get; set; }

    //To be added when smarter pathfinding is built
    //public int SearchHeuristic { get; set; }
    
    // Distance is used to track how far away this cell is from the selected cell
    int distance;

    //Unit is tracking what unit is currently set into this cell
    public Unit unit;

    #endregion
    
    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    public void SetLabel(string text)
    {
        UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }

    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;

        }
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public void UnSetNeighbor(HexDirection direction)
    {
        HexCell cell = neighbors[(int)direction];
        neighbors[(int)direction] = null;
        cell.neighbors[(int)direction.Opposite()] = null;
    }

    /*
     Disable higlight and Enable Highlight are used in selecting cells
         */
    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }
    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }




}
