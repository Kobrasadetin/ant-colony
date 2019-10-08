using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneModel : BasicModel
{
    const float PHEROMONE_RADIUS = 0.1f;
	const float FADE_OUT_TRESHOLD = 0.05f;
	const float PHERO_CONFUSE_TRSHLD = AntModel.PHERO_CONFUSE_TRSHLD;
	private float homeDistance = 0.0f;
    private float foodDistance = 0.0f;
	private float confusion = 0.0f;
	private float strength = 1.0f;
    public bool IsRepellant = false;
	public bool IsAttract = false;

	public PheromoneModel()
    {
        Radius = PHEROMONE_RADIUS;
    }

	public void SetFullStrength()
	{
		Strength = 1.0f;
	}

	public bool IsOld(){
		return strength < FADE_OUT_TRESHOLD;
	}

	public void update()
	{
		strength -= 0.0001f;
		Confusion *= 0.99f;
	}

    public float HomeDistance { get => homeDistance; set => homeDistance = value; }
    public float FoodDistance { get => foodDistance; set => foodDistance = value; }
	public float Strength { get => strength; set => strength = value; }
	public float Confusion { get => confusion; set => confusion = Mathf.Clamp(value, 0, 1); }
}
