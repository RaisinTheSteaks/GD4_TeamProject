using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RenderedPolyCount : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> renderedObjects;
    static int verts;
    static int tris;
    public TextMeshProUGUI polyStats;
    void Start()
    {
        renderedObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {


        GameObject[] ob = FindObjectsOfType<GameObject>() as GameObject[];
        foreach (GameObject rendered in ob)
        {
            if (rendered.GetComponent("Renderer") && rendered.GetComponent("MeshFilter") && rendered.name != "ImageTarget")
            {
                renderedObjects.Add(rendered);
                //print("Rendered " + rendered.name);
            }
            else if (rendered.GetComponent<SkinnedMeshRenderer>())
            {
                renderedObjects.Add(rendered);
               // print("Rendered " + rendered.name);

            }

        }
        verts = 0;
        tris = 0;


        foreach (GameObject r in renderedObjects)
        {
            var renderer = r.GetComponent<Renderer>();
            if (renderer.enabled)
            {
                if (r.GetComponent<MeshFilter>())
                {
                    MeshFilter mf = r.GetComponent<MeshFilter>();
                    tris += mf.sharedMesh.triangles.Length / 3;
                    verts += mf.sharedMesh.vertexCount;
                }
                else if (r.GetComponent<SkinnedMeshRenderer>())
                {
                    SkinnedMeshRenderer smr = r.GetComponent<SkinnedMeshRenderer>();
                    tris += smr.sharedMesh.triangles.Length / 3;
                    verts += smr.sharedMesh.vertexCount;
                }


            }


        }

        polyStats.text = "Tris: " + tris.ToString() 
            + "\n Vertex: " + verts.ToString();
        renderedObjects.Clear();
    }
}
