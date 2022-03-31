using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
public class RectAngleMesh : MonoBehaviour
{
    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        CreateMeshData();
        CreateMesh();
    }

    void CreateMeshData()
    {
        vertices = new Vector3[]
        {
            Vector3.zero,
            Vector3.forward,
            Vector3.right,
            new Vector3(1,0,1)
         };


        triangles = new int[]
        {
            0,1,2,1,3,2
        };
    }
    
    void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

}
