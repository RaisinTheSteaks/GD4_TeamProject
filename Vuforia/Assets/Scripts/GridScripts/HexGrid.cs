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

    public Color movementRangeHighlightColor;


    #endregion
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
        #region Setting the position
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f) * xOffset;
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f) * zOffset;
        #endregion

        #region Building the Cell's transform and name
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.name = "HexCell_" + x + "_" + z;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.tag = "HexCell";
        #endregion

        #region Adding the Cell's UI and Highlight components
        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        label.name = cell.name + "_Label";
        cell.uiRect = label.rectTransform;
        
        #endregion

        #region Setting the cell's Neighbors
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
        StopAllCoroutines();

        StartCoroutine(Search(fromCell, toCell, speed));
        //Search(fromCell, toCell, speed);
    }
    
    IEnumerator Search(HexCell fromCell, HexCell toCell, int speed)
    {
        /*I am setting the distance to max value to act as a check on what cell's distances haven't been gotten yet.The code will perform a breadth first search on all available cells to find the fastes route to the target cell */
        #region Setting all Hex Distances to Max int value and building the open set
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
            cells[i].SetLabel(null);
            cells[i].DisableHighlight();
        }
		fromCell.EnableHighlight(movementRangeHighlightColor);
		toCell.EnableHighlight(Color.red);

        #endregion

        WaitForSeconds delay = new WaitForSeconds(1 / 60f);
        List<HexCell> openSet = new List<HexCell>();
        fromCell.Distance = 0;
        openSet.Add(fromCell);


        #region BFS
        while (openSet.Count>0)
        {
            yield return delay;
            HexCell current = openSet[0];
            openSet.RemoveAt(0);

			if (current == toCell)
            {
                current = current.PathFrom;
                while(current!=fromCell)
                {
                    current.EnableHighlight(movementRangeHighlightColor);
                    current = current.PathFrom;
                }
                break;
            }

            //Used to figure out how far the player can go this turn
            int currentTurn = (current.Distance / speed);

            /* Hex Direction is an enum set of neighbors directions.The loop will prioritise searching in the direction of the cells neighbors, not top to bottom             */
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                //int turn = current.Distance / speed;
                HexCell neighbor = current.GetNeighbor(d);
				if (neighbor == null)
				{
					continue;
				}
                if (neighbor.Distance == int.MaxValue)
                {
                    neighbor.PathFrom = current;
                    neighbor.Distance = current.Distance + 1;
                    //neighbor.SetLabel(turn.ToString());
                    openSet.Add(neighbor);
                }
				else if(current.Distance<neighbor.Distance)
				{
                    neighbor.Distance = current.Distance;
					neighbor.PathFrom=current;
				}
               

				openSet.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            }

            


           
        }
        #endregion
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
