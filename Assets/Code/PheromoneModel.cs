using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneModel : BasicModel
{
    const float PHEROMONE_RADIUS = 0.1f;
    private float homeDistance = 0.0f;
    public PheromoneModel()
    {
        Radius = PHEROMONE_RADIUS;
    }

    public float HomeDistance { get => homeDistance; set => homeDistance = value; }
}
