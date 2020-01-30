using UnityEngine;

public class HexMapController : MonoBehaviour
{
    public HexGrid hexGrid;

    [Header("Highlights")]
    public Color highlightColor, selectedColor, movementRangeColor;
    HexCell currentCell, previousCell, moveToCell, startCell;

    [Header("Movement")]
    public int speed = 3;
    private bool isMoving = false;

    [Header("Spawning")]
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public GameObject botPrefab;
    Unit botUnit;

    private HexCoordinates spawnHex1;
    private HexCoordinates spawnHex2;

    void Awake()
    {
        //To be added for bot spawning
        //spawnHex1 = HexCoordinates.FromOffsetCoordinates(spawnPoint1.position.x, spawnPoint1.position.z);
        //spawnHex2 = HexCoordinates.FromPosition(spawnPoint2.position);
        //Debug.Log("Spawn 1:"+spawnHex1.ToString()+"  Spawn 2: "+spawnHex2.ToString());
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


        //Checking if the player has just tapped the screen
        if (Input.touchCount > 0 || Input.GetMouseButtonUp(0))
        {
            //Named tapInput as we may add in different controlls for dragging movement
            HandleTapInput();
        }
    }

    void HandleTapInput()
    {
        //If a cell has been selected, show how far away it is

        HexCell currentCell = GetCellUnderCursor();

        if (currentCell)
        {

            #region Handle input on hexagon in grid
            //if (hit.transform.gameObject.tag == "HexCell")
            //{

            if (moveToCell)
            {
                moveToCell.DisableHighlight();
                moveToCell = null;
            }
            if (isMoving)
            {
                isMoving = false;
                moveToCell = currentCell;
                moveToCell.EnableHighlight(selectedColor);
                startCell = previousCell;
            }
            else
            {
                currentCell.EnableHighlight(highlightColor);
                if (previousCell)
                    previousCell.DisableHighlight();

                previousCell = currentCell;
            }


            Debug.Log("Current Cell: " + currentCell.coordinates);
            //}
            #endregion
        }

        if (moveToCell)
        {
            hexGrid.FindPath(startCell, moveToCell, speed);
        }
    }

    public void SetMovementState(bool state)
    {
        isMoving = state;
    }

    HexCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            //not effective at filtering out objects behind UI
            //if(hit.transform.IsChildOf(hexGrid.transform))
                return hexGrid.GetCell(hit.point);
        }
        return null;
    }

    void CreateBot()
    {
        HexCell cell = GetCellUnderCursor();
        if(cell)
        {
            Unit unit=Instantiate(botUnit);
            unit.transform.SetParent(hexGrid.transform, false);
        }
    }
}