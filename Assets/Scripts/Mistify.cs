using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mistify : MonoBehaviour
{
    [Range(1f, 20f)]
    public float lerpSpeed = 1;
    public float maxBounceSpeed;

    private Material mat;
    private MistifyPoint[] points;
    private Vector2[] positions;
    private ComputeBuffer positionsBuffer;
    
    // Don't change these
    private int POINT_REPETITIONS = 5;
    private int NUMBER_OF_POINTS = 4;

    void Start()
    {
        // Bounds are set up for 16:9 display
        Vector2 xBounds = new Vector2(0f, 1f);
        Vector2 yBounds = new Vector2(3.5f / 16f, 1f - (3.5f / 16f));
        points = new MistifyPoint[NUMBER_OF_POINTS];
        for (int i = 0; i < NUMBER_OF_POINTS; i++) {
            points[i] = new MistifyPoint(xBounds, yBounds, maxBounceSpeed);
        }

        positions = new Vector2[NUMBER_OF_POINTS * POINT_REPETITIONS];
        for (int i = 0; i < NUMBER_OF_POINTS; i++) {
            Vector2 pos = points[i].GetPos();
            int offset = i * POINT_REPETITIONS;
            for (int j = 0; j < POINT_REPETITIONS; j++) {
                positions[offset + j] = pos;
            }
        }

        mat = GetComponent<MeshRenderer>().material;
        positionsBuffer = new ComputeBuffer(NUMBER_OF_POINTS * POINT_REPETITIONS, 2 * 4);
    }

    void Update()
    {
        // Move the points
        float dt = Time.deltaTime;
        foreach (MistifyPoint point in points) {
            point.Move(dt);
        }

        // Set the position values
        for (int i = 0; i < NUMBER_OF_POINTS; i++) {
            int offset = i * POINT_REPETITIONS;
            positions[offset + 0] = points[i].GetPos();
            for (int j = 1; j < POINT_REPETITIONS; j++) {
                positions[offset + j] = Vector2.Lerp(
                    positions[offset + j],
                    positions[offset + j - 1],
                    lerpSpeed * dt
                );
            }
        }

        // Dispatch the compute buffer to the shader
        positionsBuffer.SetData(positions);
        mat.SetBuffer("_Positions", positionsBuffer);
    }

    void OnDisable()
    {
		positionsBuffer.Release();
        positionsBuffer = null;
	}
}

public class MistifyPoint
{
    private Vector2 pos, dir;
    private Vector2 xBounds, yBounds;
    private float speed;
    private float maxBounceSpeed;

    // Constructor
    public MistifyPoint(Vector2 xBounds, Vector2 yBounds, float maxBounceSpeed)
    {
        this.xBounds = xBounds;
        this.yBounds = yBounds;
        this.maxBounceSpeed = maxBounceSpeed;

        pos = new Vector2(
            Random.Range(xBounds[0], xBounds[1]),
            Random.Range(yBounds[0], yBounds[1])
        );

        dir = new Vector2(
            Random.Range(0.1f, 1f),
            Random.Range(0.1f, 1f)
        ).normalized;

        speed = Random.Range(0.1f, maxBounceSpeed);
    }

    // Moves the point based on time from last frame
    // Also bounces the point and randomly chooses a new speed on collision
    public void Move(float dt)
    {
        pos += dir * speed * dt;
        if ((pos.x < xBounds[0] && dir.x < 0) || (pos.x > xBounds[1] && dir.x > 0)) {
            dir.x = -dir.x;
            speed = Random.Range(0.1f, maxBounceSpeed);
        }
        if ((pos.y < yBounds[0] && dir.y < 0) || (pos.y > yBounds[1] && dir.y > 0)) {
            dir.y = -dir.y;
            speed = Random.Range(0.1f, maxBounceSpeed);
        }
    }

    public Vector2 GetPos() { return pos; }
}
