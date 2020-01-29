using UnityEngine;

public class HexMapController : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;
    public Color highlightColor;

    HexCell currentCell, previousCell;

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
        if ((Input.touchCount>0&&Input.touchCount<3)||Input.GetMouseButtonUp(0))
        {
            HandleTapInput();
        }
    }

    void HandleTapInput()
    {

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Physics.Raycast(inputRay, out hit))
        {

            currentCell = hexGrid.GetCell(hit.point);

            if (isMoving)
            {
                currentCell.EnableHighlight(Color.red);
                isMoving = false;
            }
            else
            {
                currentCell.EnableHighlight(highlightColor);
                if (previousCell)
                    previousCell.DisableHighlight();
            }

            previousCell = currentCell;

            Debug.Log("Current Cell: " + currentCell.coordinates);
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