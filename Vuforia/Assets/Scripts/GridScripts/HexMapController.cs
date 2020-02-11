using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapController : MonoBehaviour
{
    public HexGrid hexGrid;

    [Header("Highlights")]
    public Color highlightColor, selectedColor, movementRangeColor;
    HexCell currentCell, previousCell, moveToCell, startCell;

    [Header("Movement")]
    public int speed = 2;
    private bool isMoving = false;

    [Header("Bots")]
    public Unit unitPrefab;
    Unit currentUnit;
    void Awake()
    {

    }

    void Update()
    {
        #region Notes for planning movement
        /*
         *  Need to set the Search from cell
         *      -Do when selecting bot action?
         *  Check what states are active with the action selection
         *  1) Get the action selection to set searchFromCell
         *  2) Get the bot's maximum movement
         *  3) Search all cells within the bot's movement
         *  4) Highlight a cell if
         *      a) Within Movement
         *      b) Not occupied by another bot
         *  5) If selecting movement is true 
         *          If a highlighted hex is selected
         *              Move bot to there
         *          else
         *              set selecting movement to false
         *      else
         *          set all highlights to false
         *  
         */
        #endregion

        #region Temporary Bot spawning
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateUnit();
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            DestroyUnit();
            return;
        }
        #endregion

        //Checking if the player has just tapped the screen
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Named tapInput as we may add in different controlls for dragging movement
                HandleTapInput();
                return;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            hexGrid.DoMove();
        }

    }

    void HandleTapInput()
    {
        //If a cell has been selected, show how far away it is

        HexCell currentCell = GetCellUnderCursor();

        if (currentCell)
        {
            #region Handle input on hexagon in grid

            if (moveToCell)
            {
                moveToCell.DisableHighlight();
                moveToCell = null;
            }
            if (isMoving)
            {
                isMoving = false;
                moveToCell = currentCell;
                currentCell = previousCell;
                moveToCell.EnableHighlight(selectedColor);
                startCell = previousCell;

                if (startCell.unit)
                {
                    currentUnit = startCell.unit;
                }
            }
            else
            {
                currentCell.EnableHighlight(highlightColor);
                if (previousCell)
                    previousCell.DisableHighlight();

                previousCell = currentCell;
            }
            Debug.Log("Current Cell: " + currentCell.coordinates);
           
            #endregion
        }

        if (moveToCell != null)
        {
            if (moveToCell != currentCell)
            {
                if (currentUnit)
                {
                    hexGrid.FindPath(currentUnit.Location, moveToCell, speed);
                }
            }
        }
    }

    public void SetMovementState(bool state)
    {
        isMoving = state;
    }

    public void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell)
        {
            //If the selected cell doesn't have a unit on it
            if (!cell.unit)
            {
                Unit unit = Instantiate(unitPrefab);
                unit.transform.SetParent(hexGrid.transform, false);
                unit.Location = cell;
                unit.Orientation = Random.Range(0f, 360f);
            }
        }
    }

    public void CreateUnit(HexCell cell)
    {
        //If the selected cell doesn't have a unit on it
        if (!cell.unit)
        {
            Unit unit = Instantiate(unitPrefab);
            unit.transform.SetParent(hexGrid.transform, false);
            unit.Location = cell;
            unit.Orientation = Random.Range(0f, 360f);
        }
    }

    public void CreateUnit(HexCell cell, Unit unit)
    {
        //If the selected cell doesn't have a unit on it
        if (!cell.unit)
        {
            unit.transform.SetParent(hexGrid.transform, false);
            unit.Location = cell;
            unit.Orientation = Random.Range(0f, 360f);
        }
    }
    public void DestroyUnit()
    {
        //If the selected cell has a unit in it, destroy the unit
        HexCell cell = GetCellUnderCursor();
        if(cell && cell.unit)
        {
            cell.unit.Die();
        }
    }

    HexCell GetCellUnderCursor()
    {
        return hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }
    
}