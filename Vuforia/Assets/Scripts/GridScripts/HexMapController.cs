using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapController : MonoBehaviour
{
    public HexGrid hexGrid;

    [Header("Highlights")]
    public Color highlightColor, selectedColor, movementRangeColor;
    HexCell currentCell, previousCell, moveToCell, startCell;

    [Header("Movement")]
    public static int speed = 2;
    private bool isMoving = false;
    public PlayerController playerController;
    [Header("Bots")]
    public Unit unitPrefab;
    Unit currentUnit;

    void Awake()
    {

    }

    void Update()
    {
        ////Stop the player selecting through UI components
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if(Input.touchSupported)
        {
            // HandleTouchInput();
            HandleMouseInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    void HandleTouchInput()
    {
        //Checking if the player has just tapped the screen
        if (Input.touchCount > 0)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Touch touch = Input.GetTouch(0);
                ////Named tapInput as we may add in different controlls for dragging movement
                //if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                //{
                HandleTapInput();
                //    return;
                //}
                //else
                //{
                    hexGrid.DoMove();
                    isMoving = false;
                    return;
                
            }
        }
    }

    void HandleMouseInput()
    {
        //Checking if the player has just tapped the screen
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                HandleTapInput();
                return;
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            hexGrid.DoMove();
        }
    }

    void HandleTapInput()
    {
        //If a cell has been selected, show how far away it is
        hexGrid.ClearPath();
        HexCell currentCell = GetCellUnderCursor();

        if (currentCell)
        {
            if (currentCell != previousCell)
            {
                #region Handle input on hexagon in grid
                //If the player has just moved, reset the move components
                if (moveToCell)
                {
                    moveToCell.DisableHighlight();
                    moveToCell = null;
                }
                //If the player has selected that they want to move, set the selected cell to be the move target, highlight it, 
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
                    else
                    {
                        currentUnit = null;
                    }
                }
                else
                {
                    currentCell.EnableHighlight(highlightColor);
                    if (previousCell)
                    {
                        previousCell.DisableHighlight();
                    }
                    previousCell = currentCell;
                }
                #endregion
            }
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

    public void SetSpeed(int newSpeed)
    {
        speed = newSpeed;
        hexGrid.speed = newSpeed;
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
            //unit.transform.SetParent(hexGrid.transform, false);
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