using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
Joshua Corcoran 
D00190830
_________
HexGrid.cs is used for:
Constructing the hex map,
handling the player movement input

*/


public class HexGrid : MonoBehaviour
{
    [Header ("Grid Inputs")]
    public int width = 6;
    public int height = 6;

    public float xOffset = 1;
    public float zOffset = 1;

    int cellCountX, cellCountZ;

    public HexCell cellPrefab;
    HexCell[] cells;

    public Text cellLabelPrefab;
    Canvas gridCanvas;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    //Awake is used to generate each individual tile in the level
    void Awake()
    {
        cells = new HexCell[height * width];
        gridCanvas = GetComponentInChildren<Canvas>();
        /*
         [TODO]
         Replace this construction with our own map
         -Josh 11-01

         */
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }


    //Build each given cell at these coordinates
    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f) * xOffset;
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f) * zOffset;

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.name = "HexCell_" + x + "_" + z;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();

        cell.uiRect = label.rectTransform;

        //Setting the directions of the neighboring cells
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width - 1]);
                }
            }
        }

    }

    public void TouchCell(Vector3 position)
    {
        /*
         * Currently able to select any cell on the map.
         * Doesn't do anything to the selected cell.
         */
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        Debug.Log("touched at: " + coordinates);
        
    }



    //Used to update the distance values of each cell
    public void FindPath(HexCell fromCell, HexCell toCell)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = fromCell.coordinates.DistanceTo(cells[i].coordinates);
        }
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        int x = coordinates.X + z / 2;
        return cells[x + z * width];
    }
}
