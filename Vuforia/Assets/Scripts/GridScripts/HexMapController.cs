using UnityEngine;

public class HexMapController : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;
    public Color highlightColor;

    HexCell currentCell, previousCell, searchFromCell;


    void Awake()
    {
        SelectColor(0);
    }

    void Update()
    {
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
        if (Input.OnRelease())
        {
            HandleInput();
        }
    }

    void HandleInput()
    {

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Physics.Raycast(inputRay, out hit))
        {
            if(currentCell)
            {
                currentCell.DisableHighlight();
            }
            currentCell = hexGrid.GetCell(hit.point);

            currentCell.EnableHighlight(highlightColor);
            //hexGrid.TouchCell(currentCell);

            previousCell = currentCell;

            Debug.Log("Current Cell: " + currentCell.coordinates);
        }

    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

}