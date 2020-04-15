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
    private int fingerID = -1;
    [Header("Movement")]
    public static int speed = 2;
    private bool isMoving = false;
    public PlayerController playerController;
    private bool previousClick = false;

    [Header("Bots")]
    public Unit unitPrefab;
    Unit currentUnit;
    BotController selectedBot;

    void Awake()
    {
#if !UNITY_EDITOR
            fingerID =0;
#endif
        playerController = FindObjectOfType<PlayerController>();

    }

    void Update()
    {
        
        if (!playerController)
        {
            playerController = FindObjectOfType<PlayerController>();
            return;
        }
        if (!playerController.pause)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                //Stop the player selecting through UI components
                if (!EventSystem.current.IsPointerOverGameObject(fingerID))
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
    }

    void HandleTouchInput()
    {
        //Checking if the player has just tapped the screen
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
                if(currentCell)
				{
					isMoving = true;
					PlayerController localPlayer = null;
					PlayerController[] players = FindObjectsOfType<PlayerController>();
					foreach(PlayerController player in players)
					{
						if(player.photonPlayer.IsLocal)
						{
							localPlayer = player;
						}
					}
					
					if (localPlayer.Turn)
					{
						hexGrid.FindPath(startCell, currentCell, speed - hexGrid.hexesTravelled);
					}
				}
                break;

            case TouchPhase.Ended:
                debugString = "[TOUCH PHASE ENDED]";
                previousCell = currentCell;
                currentCell = null;
                hexGrid.DoMove(selectedBot);
                isMoving = false;
                break;
        }
        if (selectedBot)
        {
            debugString = "Selected Bot";
        }
        //selectedBot.AttackTarget.text = debugString;
        return;
        
    }
    
    void HandleMouseInput()
    {
        //Checking if the player has just tapped the screen
        bool currentClick = Input.GetMouseButtonDown(0);
        if (currentClick)
        {
            string debugString = "SelectedBot with a click";
            HandleTapInput();
            previousClick = currentClick;
            if (selectedBot)
            {
                selectedBot.AttackTarget.text = debugString;
            }
            else
            {
                TextMeshProUGUI textbox = GameObject.Find("AttackDebug").transform.Find("Text").GetComponent<TextMeshProUGUI>();
                //textbox.text = "No Bots Here";
            }
            return;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            previousClick = false;
            hexGrid.DoMove(selectedBot);
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
                    hexGrid.FindPath(currentUnit.Location, endCell, speed - hexGrid.hexesTravelled);
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
        hexGrid.DisableAllHighlights();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if(hit.transform.tag == "Bot")
            {
                return hit.transform.GetComponent<Unit>().Location;
            }
        }

        HexCell cell = hexGrid.GetCell(ray);

        BotController[] bots = FindObjectsOfType<BotController>();
        int i = 0;

        if (cell)
        {
            if (cell.unit)
            {
                foreach (BotController bot in bots)
                {
                    Debug.Log("Disabling bot selection: " + bot.name + " " + i + "");
                    i++;
                    bot.isSelected = false;
                    selectedBot = null;
                }
                selectedBot = cell.unit.GetComponentInParent<BotController>();
                selectedBot.isSelected = true;
                FindObjectOfType<VoiceLineManager>().SetBotName(selectedBot.botName);
                Debug.Log("Selecting bot: " + selectedBot.name);
            }
        }
        i = 0;
        return cell;
    }

    void ClearStartingCells()
    {
        if (currentCell)
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

}