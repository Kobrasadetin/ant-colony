using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnthillModel: EntityModel
{
    private int health = 3;
	private int healthChanged;
	private float foodStorage = 99f;

    public int Health { get => health; set { HealthChanged = value-health;  health = value; } }

	public int HealthChanged { get => healthChanged; private set => healthChanged = value; }
	public float FoodStorage { get => foodStorage; }
	
	public float DecreaseFood(float amount)
	{
		float oldFood = foodStorage;
		foodStorage = Mathf.Max(foodStorage - amount, 0f);
		return oldFood - foodStorage;
	}
	public void IncreaseFood(float amount)
	{
		foodStorage += amount;
	}

	public AnthillModel() : base(0.5f)
    {

    }
	public void update() {
		HealthChanged = 0;
	}
	

}
