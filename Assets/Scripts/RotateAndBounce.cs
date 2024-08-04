using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndBounce : MonoBehaviour
{
    public Vector3 rotation;
    public Vector3 dir;
    public float speed;
    public Vector2 xBounds, yBounds, zBounds;

    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
        transform.localPosition += speed * dir * Time.deltaTime;

        // Check bounce bounds
        if (transform.localPosition.x < xBounds[0] && dir.x < 0) {
            dir.x = -dir.x;
        }
        if (transform.localPosition.x > xBounds[1] && dir.x > 0) {
            dir.x = -dir.x;
        }
        if (transform.localPosition.y < yBounds[0] && dir.y < 0) {
            dir.y = -dir.y;
        }
        if (transform.localPosition.y > yBounds[1] && dir.y > 0) {
            dir.y = -dir.y;
        }
        if (transform.localPosition.z < zBounds[0] && dir.z < 0) {
            dir.z = -dir.z;
        }
        if (transform.localPosition.z > zBounds[1] && dir.z > 0) {
            dir.z = -dir.z;
        }
    }
}
