﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicModel : IObjectModel
{
    private float radius;
    private Vector2 position;

    public float Radius { get => radius; set => radius = value; }
    public Vector2 Position { get => position; set => position = value; }

    public bool IsInside(IObjectModel largerEntity)
    {
        return Vector2.Distance(largerEntity.Position, Position) < largerEntity.Radius;
    }
}
