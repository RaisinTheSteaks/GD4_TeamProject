using UnityEngine;

public class HexMapController : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    HexCell previousCell, searchFromCell;


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
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Debug.DrawRay(inputRay.origin, inputRay.direction,Color.red);
        if (Physics.Raycast(inputRay, out hit))
        {
            hexGrid.TouchCell(hit.point);

        }
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

}