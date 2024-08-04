using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morph : MonoBehaviour
{
    public MeshFilter SphereMeshFilter, BalloonMeshFilter, FlowerMeshFilter, StretchMeshFilter;
    [Range(0.1f, 5f)]
    public float lerpSpeed = 1;

    private Vector3[] rawSphereVertices, rawBalloonVertices, rawFlowerVertices, rawStretchVertices;
    private Vector3[] cubeVertices, sphereVertices, balloonVertices, flowerVertices, stretchVertices;
    private Mesh cubeMesh;
    private float lerpPercent;
    private int LENGTH;
    
    void Start()
    {
        cubeMesh = GetComponent<MeshFilter>().mesh;
        cubeVertices = cubeMesh.vertices;
        LENGTH = cubeVertices.Length;

        rawSphereVertices = SphereMeshFilter.sharedMesh.vertices;
        rawBalloonVertices = BalloonMeshFilter.sharedMesh.vertices;
        rawFlowerVertices = FlowerMeshFilter.sharedMesh.vertices;
        rawStretchVertices = StretchMeshFilter.sharedMesh.vertices;

        sphereVertices = new Vector3[LENGTH];
        balloonVertices = new Vector3[LENGTH];
        flowerVertices = new Vector3[LENGTH];
        stretchVertices = new Vector3[LENGTH];

        lerpPercent = 0f;

        AssignEndVertices();
        StartCoroutine(BounceLerpPercent());
    }

    private void AssignEndVertices()
    {
        Vector3 cubeVertex, sphereVertex, balloonVertex, flowerVertex, stretchVertex;
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
            for (int j = 0; j < rawBalloonVertices.Length; j++)
            {
                balloonVertex = rawBalloonVertices[j];
                dot = Vector3.Dot(cubeVertex, balloonVertex) / Vector3.Magnitude(balloonVertex);
                if (dot > 0.999f)
                {
                    balloonVertices[i] = balloonVertex;
                    break;
                }
            }
            for (int j = 0; j < rawFlowerVertices.Length; j++)
            {
                flowerVertex = rawFlowerVertices[j];
                dot = Vector3.Dot(cubeVertex, flowerVertex) / Vector3.Magnitude(flowerVertex);
                if (dot > 0.999f)
                {
                    flowerVertices[i] = flowerVertex;
                    break;
                }
            }
            for (int j = 0; j < rawStretchVertices.Length; j++)
            {
                stretchVertex = rawStretchVertices[j];
                dot = Vector3.Dot(cubeVertex, stretchVertex) / Vector3.Magnitude(stretchVertex);
                if (dot > 0.999f)
                {
                    stretchVertices[i] = stretchVertex;
                    break;
                }
            }
        }
    }

    void Update()
    {
        Vector3[] lerpVertices = new Vector3[LENGTH];

        if (lerpPercent >= 2f) {
            for (int i = 0; i < LENGTH; i++) {
                lerpVertices[i] = Vector3.Lerp(balloonVertices[i], flowerVertices[i], lerpPercent - 2f);
            }
        } else if (lerpPercent >= 1f) {
            for (int i = 0; i < LENGTH; i++) {
                lerpVertices[i] = Vector3.Lerp(sphereVertices[i], balloonVertices[i], lerpPercent - 1f);
            }
        } else if (lerpPercent >= 0f) {
            for (int i = 0; i < LENGTH; i++) {
                lerpVertices[i] = Vector3.Lerp(cubeVertices[i], sphereVertices[i], lerpPercent);
            }
        } else {
            for (int i = 0; i < LENGTH; i++) {
                lerpVertices[i] = Vector3.Lerp(stretchVertices[i], cubeVertices[i], lerpPercent + 1);
            }
        }

        cubeMesh.vertices = lerpVertices;
        cubeMesh.RecalculateNormals();
    }

    IEnumerator BounceLerpPercent()
    {
        while (true)
        {
            // Increase lerpPercent
            while (lerpPercent < 3f)
            {
                lerpPercent += lerpSpeed * Time.deltaTime;
                yield return null;
            }
            // Decrease lerpPercent
            while (lerpPercent > -1f)
            {
                lerpPercent -= lerpSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }
}
