using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneModel : BasicModel
{
    const float PHEROMONE_RADIUS = 0.1f;
	const float HOME_DRIFT_SPEED = 0.001f;
	const float FOOD_DRIFT_SPEED = 0.002f;
	private float homeDistance = 0.0f;
    private float foodDistance = 0.0f;


    public PheromoneModel()
    {
        Radius = PHEROMONE_RADIUS;
    }
	public void update()
	{
		HomeDistance += HOME_DRIFT_SPEED;
		foodDistance += FOOD_DRIFT_SPEED;
	}

    public float HomeDistance { get => homeDistance; set => homeDistance = value; }
    public float FoodDistance { get => foodDistance; set => foodDistance = value; }
}
