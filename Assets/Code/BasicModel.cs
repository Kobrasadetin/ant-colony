using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicModel
{
    private float radius;
    private Vector2 position;
    private float rotation;

    public float Radius { get => radius; set => radius = value; }
    public Vector2 Position { get => position; set => position = value; }
    public float Rotation { get => rotation; set => rotation = value; }

    public BasicModel(float radius)
    {
        this.radius = radius;
    }

}
