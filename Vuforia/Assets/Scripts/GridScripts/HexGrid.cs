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
handling the player movement input,
pathfinding in player movement

*/


public class HexGrid : MonoBehaviour
{
    public GameManager gameManager;
    #region Inspector Inputs
    [Header ("Grid Inputs")]
    public int width = 6;
    public int height = 6;

    public float xOffset = 1;
    public float zOffset = 1;
    
    public HexCell cellPrefab;
    HexCell[] cells;

    public Text cellLabelPrefab;
    Canvas gridCanvas;

    [Header ("Cell Highlight Colors")]
    public Color pathHexColor;
    public Color startHexColor;
    public Color destinationHexColor;
    public Color defaultHexColor;

    [Header("Spawning")]
    public Transform[] spawnPoints;
    #endregion

    HexCell currentPathFrom, currentPathTo;
    bool currentPathExists;
    Unit selectedUnit;
    public bool HasPath
    {
        get
        {
            return currentPathExists;
        }
    }

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
        SetSpawnPoints(cells[0]);
        SetSpawnPoints(cells[cells.Length-1]);
        SetSpawnPoints(cells[7]);
        SetSpawnPoints(cells[cells.Length-7]);
    }

    

    void SetSpawnPoints(HexCell cell)
    {
        gameManager.spawnPoints[0] = cell.transform;
    }

    //Build each given cell at these coordinates
    void CreateCell(int x, int z, int i)
    {
        #region Setting the position
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f) ;
        #endregion

        #region Building the Cell's transform and name
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.name = "HexCell_" + (x - z / 2) + "_" + z;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.tag = "HexCell";
        #endregion

        #region Adding the Cell's UI and Highlight components

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = "";
        label.name = cell.name + "_Label";
        cell.uiRect = label.rectTransform;

       // cell.EnableHighlight(defaultHexColor);
        #endregion

        #region Setting the cell's Neighbors
        if(x > 0)
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
                if (x <width-1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }
        #endregion
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
    public void FindPath(HexCell fromCell, HexCell toCell, int speed)
    {
        if (fromCell.unit)
        {
            selectedUnit = fromCell.unit;
        }
        else
        {
            selectedUnit = null;
        }
        ClearPath();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists=Search(fromCell, toCell, speed);
        if(currentPathExists)
            ShowPath(speed);
    }
    
    bool Search(HexCell fromCell, HexCell toCell, int speed)
    {
        #region Setting all Hex Distances to Max int value and building the open set
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
        }

        Queue<HexCell> openSet = new Queue<HexCell>();
        fromCell.Distance = 0;
        openSet.Enqueue(fromCell);

        #endregion
        #region Breadth First Search

        while (openSet.Count>0)
        {
            HexCell current = openSet.Dequeue();
            #region Termination Condition
            if (current == toCell)
            {
                return true;
            }
            #endregion
            #region Recurse 
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
				if (neighbor != null)
				{
                    //This currently Checks all it's neighbors
                    if (neighbor.Distance == int.MaxValue)
                    {
                        neighbor.Distance = current.Distance + 1;
                        neighbor.PathFrom = current;
                        openSet.Enqueue(neighbor);
                    }
                    //To search more intelligently, we would add an else statement here to check if any other cells have a lower heuristic
                }
            }
            #endregion
        }
        return false;
        #endregion
    }

    void ShowPath(int speed)
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                int turn = current.Distance / speed;
                current.SetLabel(turn.ToString());
                current.EnableHighlight(pathHexColor);
                current = current.PathFrom;
            }
            currentPathFrom.EnableHighlight(startHexColor);
            currentPathTo.EnableHighlight(destinationHexColor);
        }
    }

    void ClearPath()
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.PathFrom;
            }
            current.DisableHighlight();
            currentPathExists = false;
        }
        else if(currentPathFrom)
        {
            currentPathFrom.DisableHighlight();
            currentPathTo.DisableHighlight();
        }
        currentPathFrom = currentPathTo = null;
    }

    void DoSelection()
    {
        if(currentPathFrom)
        {
            selectedUnit = currentPathFrom.unit;
        }
    }

    public void DoMove()
    {
        if(HasPath)
        {
            selectedUnit.Location = currentPathTo;
            ClearPath();
        }
    }

    /*
     The GetCell(Vector3) is used to get the grid cell from a click or player's world position
     The GetCell(HexCoordinates) uses a pre-defined co-ordinate to retrieve the position
     */
     
    public HexCell GetCell(Ray ray)
    {
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            return GetCell(hit.point);
        }
        return null;
    }
    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return cells[index];
    }
    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        int x = coordinates.X + z / 2;
        return cells[x + z * width];
    }
}
