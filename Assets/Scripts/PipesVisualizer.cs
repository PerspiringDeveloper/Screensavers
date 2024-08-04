using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PipesVisualizer : MonoBehaviour
{
    public GameObject straightPipe, curvedPipe, bulbPipe;
    public Material[] Colors;
    public float spacing;

    private List<GameObject> Pipes;
    private List<Material> ShuffledColors;
    private Material mat;
    private float halfX, halfY, halfZ;
    private Vector3Int dir;

    public void Initialize(int x, int y, int z)
    {
        halfX = (float)(x-1) / 2f;
        halfY = (float)(y-1) / 2f;
        halfZ = (float)(z-1) / 2f;

        dir = Vector3Int.zero;

        Pipes = new List<GameObject>();

        ShuffledColors = new List<Material>();
        List<Material> temp = new List<Material>();
        temp.AddRange(Colors);
        while (temp.Count > 0)
        {
            int index = Random.Range(0, temp.Count - 1);
            ShuffledColors.Add(temp[index]);
            temp.RemoveAt(index);
        }

        mat = ShuffledColors[0];
        ShuffledColors.RemoveAt(0);
        SetPipeMaterials(mat);
    }

    // Create a pipe piece at curPoint, directed towards newPoint
    public void DrawPipe(Vector3Int curPoint, Vector3Int newPoint)
    {
        GameObject newPipe;
        Vector3Int newDir = newPoint - curPoint;
        Vector3 pos = new Vector3(
            spacing * (curPoint.x - halfX),
            spacing * (curPoint.y - halfY),
            spacing * (curPoint.z - halfZ)
        );

        // If this is the first pipe piece
        if (dir.magnitude < 0.1f) {
            dir = newDir;
            DrawEndCap(pos, dir);
            return;
        }

        // If this is the last pipe piece
        if (newDir.magnitude < 0.1f) {
            DrawEndCap(pos, -dir);
            dir = Vector3Int.zero;
            return;
        }

        // If we haven't changed direction, draw a straight pipe piece
        if (newDir.x * dir.x + newDir.y * dir.y + newDir.z * dir.z > 0.9) {
            newPipe = Instantiate(
                straightPipe, pos, Quaternion.FromToRotation(Vector3Int.forward, dir), this.transform
            );
            newPipe.transform.localPosition = pos;
            Pipes.Add(newPipe);
            return;
        }

        // If we've changed direction, figure out the rotation of the curved pipe piece
        Vector3Int pipeDir = newDir - dir;
        Quaternion rot = Quaternion.LookRotation(pipeDir, Vector3.up);
        if (pipeDir.y == 0) {
            rot *= Quaternion.AngleAxis(90, Vector3.forward);
        }

        // Draw the curved pipe
        newPipe = Instantiate(curvedPipe, pos, rot, this.transform);
        newPipe.transform.localPosition = pos;
        Pipes.Add(newPipe);

        // Update dir and return
        dir = newDir;
        return;
    }

    private void DrawEndCap(Vector3 pos, Vector3Int dir)
    {
        if (dir.magnitude < 0.1) { return; }
        GameObject newPipe = Instantiate(
            bulbPipe, pos, Quaternion.FromToRotation(Vector3Int.forward, dir), this.transform
        );
        newPipe.transform.localPosition = pos;
        Pipes.Add(newPipe);
        return;
    }

    public int GetColorCount()
    {
        return ShuffledColors.Count;
    }

    public void GetNewColor()
    {
        if (GetColorCount() == 0) { return ; }
        mat = ShuffledColors[0];
        ShuffledColors.RemoveAt(0);
        SetPipeMaterials(mat);
    }

    private void SetPipeMaterials(Material mat)
    {
        straightPipe.GetComponent<MeshRenderer>().material = mat;
        curvedPipe.GetComponent<MeshRenderer>().material = mat;
        bulbPipe.GetComponent<MeshRenderer>().material = mat;
    }

    public void DestroyPipes()
    {
        while (Pipes.Count > 0) {
            Destroy(Pipes[0]);
            Pipes.RemoveAt(0);
        }
    }

    // Old Stuff
    public GameObject cube;
    public void DrawCube(Vector3Int point)
    {
        Vector3 pos = new Vector3(
            spacing * (point.x - halfX),
            spacing * (point.y - halfY),
            spacing * (point.z - halfZ)
        );

        GameObject newCube = Instantiate(cube, this.transform);
        newCube.transform.localPosition = pos;
    }

}
