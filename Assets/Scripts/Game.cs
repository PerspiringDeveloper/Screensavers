using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject PipesObject;
    public MeshRenderer MistifyMesh1, MistifyMesh2, FlowerBoxMesh, StardustMesh;

    public Stardust stardust;
    private float height, width;

    void Start()
    {
        // Start with FlowerBox, the others will be off
        PipesObject.GetComponent<Pipes>().enabled = false;
        StardustMesh.enabled = false;
        MistifyMesh1.enabled = false;
        MistifyMesh2.enabled = false;

        height = Screen.height;
        width = Screen.width;

        Cursor.visible = false;
    }

    void Update()
    {
        // Turn flowerbox on
        if (Input.GetKeyDown("1")) {
            FlowerBoxMesh.enabled = true;
            StardustMesh.enabled = false;
            PipesObject.GetComponent<Pipes>().enabled = false;
            MistifyMesh1.enabled = false;
            MistifyMesh2.enabled = false;
        }
        // Turn stardust on
        if (Input.GetKeyDown("2")) {
            FlowerBoxMesh.enabled = false;
            StardustMesh.enabled = true;
            PipesObject.GetComponent<Pipes>().enabled = false;
            MistifyMesh1.enabled = false;
            MistifyMesh2.enabled = false;
        }
        // Turn pipes on
        if (Input.GetKeyDown("3")) {
            FlowerBoxMesh.enabled = false;
            StardustMesh.enabled = false;
            PipesObject.GetComponent<Pipes>().enabled = true;
            MistifyMesh1.enabled = false;
            MistifyMesh2.enabled = false;
        }
        // Turn mistify on
        if (Input.GetKeyDown("4")) {
            FlowerBoxMesh.enabled = false;
            StardustMesh.enabled = false;
            PipesObject.GetComponent<Pipes>().enabled = false;
            MistifyMesh1.enabled = true;
            MistifyMesh2.enabled = true;
        }

        // Set the mouse position for the stardust shader
        Vector3 mousePos = Input.mousePosition;
        Vector2 mouseScreen = new Vector2(mousePos.x / width - 0.5f, mousePos.y / height - 0.5f);
        stardust.SetMouse(mouseScreen);
    }
}
