using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldMorph : MonoBehaviour
{
    public MeshFilter SphereMeshFilter;
    [Range(0f, 1f)]
    public float lerpPercent = 0;

    private Vector3[] rawSphereVertices;
    private Vector3[] cubeVertices, sphereVertices;
    private Mesh cubeMesh;
    private int LENGTH;
    
    void Start()
    {
        cubeMesh = GetComponent<MeshFilter>().mesh;
        cubeVertices = cubeMesh.vertices;
        LENGTH = cubeVertices.Length;

        rawSphereVertices = SphereMeshFilter.sharedMesh.vertices;

        sphereVertices = new Vector3[LENGTH];

        lerpPercent = 0f;

        AssignEndVertices();
    }

    private void AssignEndVertices()
    {
        Vector3 cubeVertex, sphereVertex;
        float dot;

        for (int i = 0; i < LENGTH; i++)
        {
            cubeVertex = cubeVertices[i].normalized;
            for (int j = 0; j < rawSphereVertices.Length; j++)
            {
                sphereVertex = rawSphereVertices[j];
                dot = Vector3.Dot(cubeVertex, sphereVertex) / Vector3.Magnitude(sphereVertex);
                if (dot > 0.999f)
                {
                    sphereVertices[i] = sphereVertex;
                    break;
                }
            }
        }
    }

    void Update()
    {
        Vector3[] lerpVertices = new Vector3[LENGTH];
        
        for (int i = 0; i < LENGTH; i++) {
            lerpVertices[i] = Vector3.Lerp(cubeVertices[i], sphereVertices[i], lerpPercent);
        }

        cubeMesh.vertices = lerpVertices;
        cubeMesh.RecalculateNormals();
    }
}
