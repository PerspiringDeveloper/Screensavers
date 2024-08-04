using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipes : MonoBehaviour
{
    public int xSize, ySize, zSize;
    public MeshRenderer BlackQuad;

    private PipesVisualizer vis;
    private Vector3Int curPoint;
    
    // First position is top-left corner of the cube at closest depth relative to the camera
    // x goes left to right, then y goes top to bottom, then z goes front to back
    private int[] positions;

    void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        vis = GetComponent<PipesVisualizer>();
        vis.Initialize(xSize, ySize, zSize);

        positions = new int[xSize * ySize * zSize];

        curPoint.x = Random.Range(0, xSize);
        curPoint.y = Random.Range(0, ySize);
        curPoint.z = Random.Range(0, zSize);

        SetPosition(curPoint);
        StartCoroutine(PipeRoutine());
    }

    IEnumerator PipeRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);
        bool allowedToDraw = true;
        while (allowedToDraw)
        {
            Vector3Int newPoint = PickNewPoint();

            // If that was an invalid point, try to start a new pipe
            while (newPoint.x == -1) {
                vis.DrawPipe(curPoint, curPoint);
                curPoint = StartNewPipe();

                // If there's no more available pipes / spots, break out of the coroutine
                if (curPoint.x == -1) {
                    allowedToDraw = false;
                    StartCoroutine(FadeAndRestart());
                    break;
                }

                vis.GetNewColor();
                SetPosition(curPoint);
                newPoint = PickNewPoint();
            }

            // Draw a pipe piece at curPoint heading to newPoint
            if (allowedToDraw) {
                vis.DrawPipe(curPoint, newPoint);
                curPoint = newPoint;
                SetPosition(curPoint);
            }

            yield return wait;
        }
    }

    // Finds an adjacent spot for the next pipe, returns (-1,-1,-1) if none are available
    private Vector3Int PickNewPoint()
    {
        List<Vector3Int> potentialPos = new List<Vector3Int>();
        Vector3Int nextPoint;

        nextPoint = curPoint + new Vector3Int(1, 0, 0);
        if (nextPoint.x < xSize && GetPosition(nextPoint) == 0) {
            potentialPos.Add(nextPoint);
        }
        nextPoint = curPoint + new Vector3Int(-1, 0, 0);
        if (nextPoint.x > -1 && GetPosition(nextPoint) == 0) {
            potentialPos.Add(nextPoint);
        }
        nextPoint = curPoint + new Vector3Int(0, 1, 0);
        if (nextPoint.y < ySize && GetPosition(nextPoint) == 0) {
            potentialPos.Add(nextPoint);
        }
        nextPoint = curPoint + new Vector3Int(0, -1, 0);
        if (nextPoint.y > -1 && GetPosition(nextPoint) == 0) {
            potentialPos.Add(nextPoint);
        }
        nextPoint = curPoint + new Vector3Int(0, 0, 1);
        if (nextPoint.z < zSize && GetPosition(nextPoint) == 0) {
            potentialPos.Add(nextPoint);
        }
        nextPoint = curPoint + new Vector3Int(0, 0, -1);
        if (nextPoint.z > -1 && GetPosition(nextPoint) == 0) {
            potentialPos.Add(nextPoint);
        }

        if (potentialPos.Count == 0) {
            return new Vector3Int(-1, -1, -1);
        }

        return potentialPos[Random.Range(0, potentialPos.Count)];
    }

    // Tries to start a new pipe, stops the coroutine if it can't
    private Vector3Int StartNewPipe()
    {
        // Check if there are spots available in positions
        bool spotsAvailable = false;
        foreach (int val in positions) {
            if (val == 0) {
                spotsAvailable = true;
                break;
            }
        }

        // If we can't continue building a new pipe, return
        if (!spotsAvailable || vis.GetColorCount() == 0) {
            return Vector3Int.one * -1;
        }

        // Find a spot to begin the next pipe
        int startIndex = Random.Range(0, positions.Length);
        while (positions[startIndex] == 1) {
            startIndex++;
            if (startIndex >= positions.Length) {
                startIndex = 0;
            }
        }

        // Calculate the new pipe position and return it
        Vector3Int ret = Vector3Int.zero;
        ret.z = startIndex / (xSize * ySize);
        startIndex -= ret.z * xSize * ySize;
        ret.y = startIndex / (xSize);
        ret.x = startIndex - (ret.y * xSize);
        return ret;
    }

    IEnumerator FadeAndRestart()
    {
        // Fade in a black quad
        BlackQuad.enabled = true;
        float time = 0f;
        while (time < 3)
        {
            time += Time.deltaTime;
            Color color = Color.black;
            color.a = Mathf.Min(1f, time / 2f);
            BlackQuad.material.color = color;
            yield return null;
        }
        BlackQuad.enabled = false;

        // Restart everything
        vis.DestroyPipes();
        Initialize();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        BlackQuad.enabled = false;
        vis.DestroyPipes();
    }

    private void SetPosition(Vector3Int pos)
    {
        int xOffset = pos.x;
        int yOffset = pos.y * xSize;
        int zOffset = pos.z * xSize * ySize;
        if (positions[xOffset + yOffset + zOffset] == 1) {
            Debug.Log("Error: Just set a position that was already occupied!");
        }
        positions[xOffset + yOffset + zOffset] = 1;
    }

    private int GetPosition(Vector3Int pos)
    {
        int xOffset = pos.x;
        int yOffset = pos.y * xSize;
        int zOffset = pos.z * xSize * ySize;
        return positions[xOffset + yOffset + zOffset];
    }
}
