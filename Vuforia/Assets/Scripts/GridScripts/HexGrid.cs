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
        for(int z=0,i=0;z<height;z++)
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
        position.x = (x + z*0.5f - z/2) * (HexMetrics.innerRadius*2f)*xOffset;
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f)*zOffset;

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.name="HexCell_"+x+"_"+z;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();

    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
            TouchCell(hit.point);
    }

    void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at: " + coordinates);
    }

}
