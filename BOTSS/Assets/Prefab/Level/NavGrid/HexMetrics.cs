using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    //Outer radius is the distance from the centre to the corners
    public const float outerRadius = 10f;
    //Distance from centre to perpendicular edge
    public const float innerRadius = outerRadius * 0.866025404f;
    //The vertex edges of the hexagon
    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
    };
}
