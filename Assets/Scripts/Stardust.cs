using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stardust : MonoBehaviour
{
    public Material starMat;

    public void SetMouse(Vector2 mouseScreen)
    {
        starMat.SetVector("_Mouse", mouseScreen);
    }
}
