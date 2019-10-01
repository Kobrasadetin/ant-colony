using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicModel
{
    private float radius;
    private Vector2 position;
    private float rotation;
    private BasicModel carriedBy = null;
    private BasicModel carrying = null;

    public float Radius { get => radius; set => radius = value; }
    public Vector2 Position { get => position; set => position = value; }
    public float Rotation { get => rotation; set => rotation = value; }
    public BasicModel CarriedBy
    {
        get => carriedBy; set
        {
            if (CarriedBy != null)
            {
                CarriedBy.Carrying = null;
            }
            carriedBy = value;
        }
    }

    public BasicModel Carrying { get => carrying; set => carrying = value; }

    public BasicModel(float radius)
    {
        this.radius = radius;
    }

}
