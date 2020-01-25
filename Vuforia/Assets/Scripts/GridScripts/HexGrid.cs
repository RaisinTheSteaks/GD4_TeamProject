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
    public int width = 6;
    public int height = 6;

    public float xOffset = 1;
    public float zOffset = 1;

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

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Debug.DrawRay(inputRay.origin, inputRay.direction,Color.red);
        if (Physics.Raycast(inputRay, out hit))
        {
            TouchCell(hit.point);
        }
    }

    void TouchCell(Vector3 position)
    {
        /*
         *Known issue with the touching cells.
         *As the grid gets wider/ further from origin, rounding errors in
         *   from position start to crop up more and they aren't being adjusted.
         *   There is a correlation between the radius set down in the 
         *       HexMetrics.cs and the rounding error
         *   I suspect that there is some incongruity between the hex metrics and
         *       the size of the mesh being used thats causing the issue. 
         *   -Josh 12-01-20
         *
         */

        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        Debug.Log("touched at: " + coordinates);
    }

    //Used to update the distance values of each cell
    public void FindDistancesTo(HexCell cell)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = cell.coordinates.DistanceTo(cells[i].coordinates);
        }
    }
}
