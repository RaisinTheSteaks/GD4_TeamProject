using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public Color color;
    public RectTransform uiRect;
    [SerializeField]
    HexCell[] neighbors;

    public HexCell PathFrom { get; set; }

    /*
     * Distance is used to track how far away this cell is from the selected cell
     */
    int distance;
    
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
