using System;
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

struct CoverHex
{
    public HexCell cell;
    public HexDirection direction;
}

public class HexGrid : MonoBehaviour
{
    #region Inspector Inputs
    public GameManager gameManager;
    [Header("Rotation")]
    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;
    [SerializeField]
    [Range(0f,1f)]
    private float lerpPct = 0.5f;

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
    public Color pathHexColor, startHexColor, destinationHexColor, defaultHexColor;

    [Header("Spawning")]
    public HexCell[] spawnPoints;

    public float directionRotationFraction = (360f/(int)HexDirection.HexDirectionCount);
    public Cover coverPrefab;
    //If I want to add these cover hexes in the inspector, I would add it here.
    //public string[] coverHexNames;
    public static int coverObjectCount=14;
    CoverHex[] coverHexes = new CoverHex[coverObjectCount];
    #endregion

    //Used for movement
    HexCell currentPathFrom, currentPathTo;
    bool currentPathExists;
    Unit selectedUnit;
    public int hexesTravelled = 0;
    public int speed = 0;
    public bool HasPath
    {
        get
        {
            return currentPathExists;
        }
    }

    //Awake is used to generate each individual tile in the level and set the spawn points
    void Awake()
    {
        cells = new HexCell[height * width];
        gridCanvas = GetComponentInChildren<Canvas>();
        /*
         [TODO]
         Replace this construction with our own map
         -Josh 01-11

         */
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
        SetSpawnPoints();
        SetCoverHexes();
        SpawnCover();
    }

    void Update()
    {
        //LerpRotPos();
    }
    
    void SetSpawnPoints()
    {
        spawnPoints[0] = cells[0];
        spawnPoints[1] = cells[4];
        spawnPoints[2] = cells[cells.Length-2];
        spawnPoints[3] = cells[cells.Length-6];
        gameManager.spawnPoints = spawnPoints;
    }

    void SetCoverHexes()
    {
        //Currently not a great approach, too many magic numbers, should be refactored to allow for easier inspector modification

        CoverHex coverHex = new CoverHex
        {
            cell = cells[1],
            direction = HexDirection.NE
        };
        coverHexes[0] = coverHex;
        
        coverHex.cell = cells[8];
        coverHex.direction = HexDirection.NE;
        coverHexes[1] = coverHex;

        coverHex.cell = cells[11];
        coverHex.direction = HexDirection.NW;
        coverHexes[2] = coverHex;

        coverHex.cell = cells[13];
        coverHex.direction = HexDirection.E;
        coverHexes[3] = coverHex;

        coverHex.cell = cells[20];
        coverHex.direction = HexDirection.SW;
        coverHexes[4] = coverHex;

        coverHex.cell = cells[25];
        coverHex.direction = HexDirection.NE;
        coverHexes[5] = coverHex;

        coverHex.cell = cells[29];
        coverHex.direction = HexDirection.E;
        coverHexes[6] = coverHex;

        coverHex.cell = cells[33];
        coverHex.direction = HexDirection.E;
        coverHexes[7] = coverHex;

        coverHex.cell = cells[38];
        coverHex.direction = HexDirection.SW;
        coverHexes[8] = coverHex;

        coverHex.cell = cells[43];
        coverHex.direction = HexDirection.NW;
        coverHexes[9] = coverHex;

        coverHex.cell = cells[46];
        coverHex.direction = HexDirection.NE;
        coverHexes[10] = coverHex;

        coverHex.cell = cells[49];
        coverHex.direction = HexDirection.E;
        coverHexes[11] = coverHex;

        coverHex.cell = cells[52];
        coverHex.direction = HexDirection.SW;
        coverHexes[12] = coverHex;

        coverHex.cell = cells[54];
        coverHex.direction = HexDirection.NE;
        coverHexes[13] = coverHex;
    }

    void SpawnCover()
    {
        foreach(CoverHex coverHex in coverHexes)
        {
            Cover cover = Instantiate<Cover>(coverPrefab);
            cover.ParentCell = coverHex.cell;
            cover.direction = coverHex.direction;

            //Set the scale of the object and it's parent to be it's host cell
            cover.transform.parent = cover.ParentCell.transform;
            cover.transform.localScale = new Vector3(2, 20, 2);
            cover.name = cover.ParentCell.name + "_Cover";

            //Rotate the hex to face the set direction 
            float rotationAngle = ((int)(cover.direction) * directionRotationFraction);
            cover.transform.Rotate(new Vector3(0, 1, 0), rotationAngle);

            //Spawn the cover on it's parent cell
            cover.transform.position = coverHex.cell.transform.position;

            //Set it's position to be off from the centre of the hex to the edge it is facing
            cover.transform.position += (cover.transform.forward * 0.007f);

            //Adjust spawn position
            Vector3 offset = Vector3.zero; 
            float offsetScale = 0.004f;
            switch (cover.direction)
            {
                //Used to adjust where it spawns as the current system leaves each object slightly out of position
                case (HexDirection.E):
                    offset.z = -0.8f * offsetScale;
                    break;
                case (HexDirection.NE):
                    offset.x = 0.6f * offsetScale;
                    offset.z = -0.7f * offsetScale;
                    break;
                case (HexDirection.NW):
                    offset.z = 0.7f * offsetScale;
                    offset.x = 0.6f * offsetScale;
                    break;
                case (HexDirection.SE):
                    offset.x = -0.6f * offsetScale;
                    offset.z = -0.25f * offsetScale;
                    break;
                case (HexDirection.SW):
                    offset.x = -1f * offsetScale;
                    offset.z = 0.5f * offsetScale;
                    break;
                case (HexDirection.W):
                    offset.z = 1 * offsetScale;
                    offset.x = -0.5f * offsetScale;
                    break;
            }
            cover.transform.position += offset;

            cover.ParentCell.UnSetNeighbor(cover.direction);
            //Debug.Log("Removing " + cover.ParentCell.name + "'s neighbor: " + cover.ParentCell.GetNeighbor(cover.direction) + " in direction: " + cover.direction);
        }
    }
    
    //Build each given cell at these coordinates
    void CreateCell(int x, int z, int i)
    {
        #region Setting the position
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f * xOffset);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f * zOffset) ;
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
        currentPathExists = Search(fromCell, toCell, speed);
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
        bool pathfound = false;
        while (openSet.Count>0)
        {
            HexCell current = openSet.Dequeue();
            #region Termination Condition
            if (current == toCell)
            {
                pathfound = true;
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
                        if(neighbor.Distance == speed-1)
                        {
                            neighbor.EnableHighlight(defaultHexColor);
                        }
                        neighbor.PathFrom = current;
                        openSet.Enqueue(neighbor);
                    }
                    //To search more intelligently, we would add an else statement here to check if any other cells have a lower heuristic
                }
            }
            #endregion
        }
        return pathfound;
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
                if (turn <= 1)
                {
                    current.inRange = true;
                }
                current = current.PathFrom;
            }
            currentPathFrom.EnableHighlight(startHexColor);
            currentPathTo.EnableHighlight(destinationHexColor);
        }
    }

    public void DisableAllHighlights()
    {
        foreach(HexCell cell in cells)
        {
            cell.DisableHighlight();
        }
    }

    public void ClearPath()
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                //current.DisableHighlight();
                current = current.PathFrom;
            }
            //current.DisableHighlight();
            currentPathExists = false;
        }
        else if(currentPathFrom)
        {
            currentPathFrom.DisableHighlight();
            currentPathFrom.SetLabel(null);
            currentPathTo.DisableHighlight();
            currentPathFrom.SetLabel(null);
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

    public void DoMove(BotController selectedBot)
    {

            if (HasPath)
            {
                if (currentPathTo.inRange == true)
                {
                    if (currentPathTo.Distance < (speed - hexesTravelled))
                    {
                        if (!currentPathTo.unit)
                        {
                            selectedUnit.Location = currentPathTo;
                            hexesTravelled += currentPathTo.Distance;
                        }
                        selectedBot.transform.parent.GetComponent<PlayerController>().actionCount++;
                        selectedBot.transform.parent.GetComponent<PlayerController>().botHasMoved = true;
                    }
                }
            }
        //}
        DisableAllHighlights();
        ClearPath();
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

        //if(coordinates.X > height)
        //{
        //    return null;
        //}
        //if (coordinates.Z > width)
        //{
        //    return null;
        //}
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
