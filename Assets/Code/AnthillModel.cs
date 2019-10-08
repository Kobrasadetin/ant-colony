using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnthillModel: EntityModel
{
	private int buildingBlocks = 0;
    private int happiness = 0;
	private int happinessChanged;
	private float foodStorage = 99f;

    public int Happiness { get => happiness; set { HappinessChange = value-happiness;  happiness = value; } }

	public int HappinessChange { get => happinessChanged; private set => happinessChanged = value; }
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
		HappinessChange = 0;
	}
	

}
