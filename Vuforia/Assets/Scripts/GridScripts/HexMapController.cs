using UnityEngine;

public class HexMapController : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    public Color highlightColor;
    public Color selectedColor;
    public Color movementRangeColor;
    HexCell currentCell, previousCell, moveToCell;
   
    private Color activeColor;
    private bool isMoving=false;


    void Awake()
    {
        SelectColor(0);
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
        if (Input.touchCount>0||Input.GetMouseButtonUp(0))
        {
            HandleTapInput();
        }
    }

    void HandleTapInput()
    {
        //If a cell has been selected, show how far away it is

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {

            #region Handle input on hexagon in grid
                //if (hit.transform.gameObject.tag == "HexCell")
                //{
                currentCell = hexGrid.GetCell(hit.point);

                if (moveToCell)
                    moveToCell.DisableHighlight();

                if (isMoving)
                {
                    isMoving = false;
                    moveToCell = currentCell;
                    moveToCell.EnableHighlight(selectedColor);
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

        if (currentCell)
        {
            hexGrid.FindDistancesTo(currentCell);
        }
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

    public void SetMovementState(bool state)
    {
        isMoving = state;
    }
}