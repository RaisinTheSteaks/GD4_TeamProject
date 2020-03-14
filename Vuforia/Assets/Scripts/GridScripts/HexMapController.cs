using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapController : MonoBehaviour
{
    public HexGrid hexGrid;

    [Header("Highlights")]
    public Color highlightColor, selectedColor, movementRangeColor;
    HexCell currentCell, previousCell, endCell, startCell;

    [Header("Movement")]
    public static int speed = 2;
    private bool isMoving = false;
    public PlayerController playerController;

    [Header("Bots")]
    public Unit unitPrefab;
    Unit currentUnit;
    BotController selectedBot;
    void Awake()
    {

    }

    void Update()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            ////Stop the player selecting through UI components
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.touchSupported)
                {
                    HandleTouchInput();
                }
                else
                {
                    HandleMouseInput();
                }
            }
        }
    }

    void HandleTouchInput()
    {
        //Checking if the player has just tapped the screen
        if (Input.touchCount > 0)
        {
            string debugString = "SelectedBot with a touch";
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    debugString = "[TOUCH PHASE BEGAN]";

                    ClearStartingCells();
                    hexGrid.ClearPath();

                    //Select Cell
                    HexCell cell = GetCellUnderCursor();
                    if (cell)
                    {
                        cell.EnableHighlight(hexGrid.startHexColor);
                        if (cell.unit)
                        {
                            startCell = cell;
                            currentCell = startCell;
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    debugString = "[TOUCH PHASE MOVED]";
                    previousCell = currentCell;
                    currentCell = GetCellUnderCursor();
                    isMoving = true;
                    hexGrid.FindPath(startCell, currentCell, speed);
                    break;

                case TouchPhase.Ended:
                    debugString = "[TOUCH PHASE ENDED]";
                    previousCell = currentCell;
                    currentCell = null;
                    hexGrid.DoMove();
                    isMoving = false;
                    break;
            }
            if (selectedBot)
            {
                debugString = "Selected Bot";
            }
            selectedBot.AttackTarget.text = debugString;
            //HandleTapInput();
            //hexGrid.DoMove();
            //isMoving = false;
            return;
        }
    }

    void ClearStartingCells()
    {
        if(currentCell)
        {
            currentCell.DisableHighlight();
            currentCell = null;
        }
        if (startCell)
        {
            startCell.DisableHighlight();
            startCell = null;
        }
        if (endCell)
        {
            endCell.DisableHighlight();
            endCell = null;
        }
        if (previousCell)
        {
            previousCell.DisableHighlight();
        }
    }

    void HandleMouseInput()
    {
        //Checking if the player has just tapped the screen
        if (Input.GetMouseButtonDown(0))
        {
            string debugString = "SelectedBot with a click";
            HandleTapInput();

            if (selectedBot)
            {
                selectedBot.AttackTarget.text = debugString;
            }
            else
            {
                TextMeshProUGUI textbox = GameObject.Find("AttackDebug").transform.Find("Text").GetComponent<TextMeshProUGUI>();
                textbox.text = "No Bots Here";
            }
            return;
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
        currentCell = GetCellUnderCursor();

        if (currentCell)
        {
            if (currentCell != previousCell)
            {
                #region Handle input on hexagon in grid
                //If the player has just moved, reset the move components
                if (endCell)
                {
                    endCell.DisableHighlight();
                    endCell = null;
                }
                //If the player has selected that they want to move, set the selected cell to be the move target, highlight it, 
                if (isMoving)
                {
                    isMoving = false;
                    endCell = currentCell;
                    currentCell = previousCell;
                    endCell.EnableHighlight(selectedColor);
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
        if (endCell != null)
        {
            if (endCell != currentCell)
            {
                if (currentUnit)
                {
                    hexGrid.FindPath(currentUnit.Location, endCell, speed);
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
                unit.Orientation = UnityEngine.Random.Range(0f, 360f);
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
            unit.Orientation = UnityEngine.Random.Range(0f, 360f);
        }
    }

    public void CreateUnit(HexCell cell, Unit unit)
    {
        //If the selected cell doesn't have a unit on it
        if (!cell.unit)
        {
            unit.Location = cell;
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
        HexCell cell = hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));

        BotController[] bots = FindObjectsOfType<BotController>();
        foreach (BotController bot in bots)
        {
            bot.isSelected = false;
            selectedBot = null;
        }

        if (cell.unit)
        {
            selectedBot = cell.unit.GetComponentInParent<BotController>();
            selectedBot.isSelected = true;
        }
        return cell;
    }
}