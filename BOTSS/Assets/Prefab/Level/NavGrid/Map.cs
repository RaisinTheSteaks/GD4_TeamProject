using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    //These are the inputs from the engine
    public GameObject hexPrefab;    //Spawning object
    public int radius;              //Number of hexes out
    
    public float xOffset;           //Space between hexes in the x and z direction
    public float zOffset;

    void Start()
    {
        PrintMap();
    }

    void PrintMap()
    {
        //Used as the start point from which all of the tiles will be spawned
        Vector3 origin= this.transform.position;

        //Print each quarter of the map row by row

        //TopRight
        PrintQuarter(1,1, origin);
        //TopLeft
        PrintQuarter(-1,1, origin);
        //BottomRight
        PrintQuarter(1,-1, origin);
        //BottomLeft
        PrintQuarter(-1,-1, origin);
    }

    void PrintQuarter(int xDirection, int zDirection, Vector3 origin)
    {
        
        
        for (int z = 0; z<(radius*zDirection); z += zDirection)
        {
            Vector3 rowStart = origin;
            //If An odd numbered row, offset the row
            if (z % 2 == 1)
            {
                rowStart.x += xOffset / 2;
            }

            

            PrintRow(hexPrefab, radius, z, xDirection, rowStart);
        }
    }

    //Print count is used to check what position in the z is printed
    void PrintRow(GameObject baseShape, int numInRow, int rowNum, int positive, Vector3 position)
    {
        Vector3 hexPos = position;
        for (int i=0;i<numInRow;i++)
        {
            float xPos = i * xOffset;
            hexPos.x = xPos;
            PrintHex(baseShape, hexPos, i, rowNum);
        }
    }

    void PrintHex(GameObject baseShape, Vector3 position, int x, int z)
    {
        GameObject hex_go = (GameObject)Instantiate(hexPrefab, position, Quaternion.identity);
        hex_go.name = "Hex_" + x + "_" + z;
        hex_go.transform.SetParent(this.transform);
        hex_go.isStatic = true;
    }
}
//void Start()
//    {
//        for (int x = 0; x < width; x++)
//        {
//            for (int y = 0; y < height; y++)
//            {
//                float xPos = x * xOffset;
//                if (y % 2 == 1)
//                {
//                    xPos += xOffset / 2;
//                }
//                GameObject hex_go = (GameObject)Instantiate(
//                    hexPrefab, 
//                    new Vector3(xPos, 0, y * zOffset), 
//                    Quaternion.identity);
//                hex_go.name = "Hex_" + x + "_" + y;
//                hex_go.transform.SetParent(this.transform);
//                hex_go.isStatic = true;
//            }
//        }
//    }
//}
