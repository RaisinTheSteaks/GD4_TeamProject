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
        //Debug.Log("Starting to build map");
        PrintMap();
    }

    void PrintMap()
    {
        /*
         1) Print origin Line
         2) Print r rows in positive direction where each successive row is 1 hex smaller than the last
            [a] Offset each odd row by half a hex
            [b] Move each row's start position half a hex forward in the x
         */
         
        int numInRow= (radius*2)+1;
        bool oddRow =false;
        Vector3 rowStart= this.transform.position;
        bool boolx = false;
        int rowCount = radius*2;
        PrintSection(1, 1, numInRow, oddRow, rowStart, boolx, rowCount);
    }
    static int printCount = 0;

    void PrintSection(int x, int z, int numInRow, bool oddRow, Vector3 rowStart, bool boolx, int rowCount)
    {
        for (int i = 0; i < rowCount; i++)
        {
            float zPos = zOffset * i * z;

            //If An odd numbered row, offset the row
            if (i % 2 == 1)
            {
                oddRow = true;
                rowStart.x += xOffset / 2;
            }
            else
            {
                oddRow = false;
            }

            rowStart.x += xOffset / 2;
            rowStart.z = zPos;
            PrintRow(hexPrefab, numInRow, oddRow, z, rowStart);
            numInRow--;
        }
    }

    void PrintRow(GameObject baseShape, int numInRow, bool oddRowCount, int positive, Vector3 position)
    {
        Vector3 hexPos = position;
        for (int i=0;i<numInRow;i++)
        {
            float xPos = i * xOffset;
            if (oddRowCount)
            {
                xPos += xOffset / 2;
            }
            hexPos.x = xPos;
            PrintHex(baseShape, hexPos, i,printCount);
        }
        printCount++;
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
