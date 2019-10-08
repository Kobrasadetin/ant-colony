using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodModel : EntityModel
{
    private float nutritionValue;
    private float poisonValue;
	public const float DEFAULT_RADIUS = 0.05f;
	public Type type;
	public enum Type
	{
		NONE,
		BREAD,
		DEAD_ANT
	}

	public static FoodModel DeadAnt(AntModel fromAnt){
		return new FoodModel(Type.DEAD_ANT, fromAnt.Position, DEFAULT_RADIUS, 1.0f - fromAnt.Hunger + 0.1f, fromAnt.Poison);
	}

	public FoodModel(Type type, Vector2 position, float radius, float nutritionValue, float poisonValue) : base(radius)
	{
		this.type = type;
		this.nutritionValue = nutritionValue;
		this.poisonValue = poisonValue;
		this.Position = new Vector2(position.x, position.y);
	}

	public FoodModel(Vector2 position, float radius) : this(Type.BREAD, position, radius, 1.0f, 0f)
	{
    }

	public float DecreaseNutrition(float amount){
		float oldNutrition = nutritionValue;
		nutritionValue = Mathf.Max(nutritionValue - amount, 0f);
		return oldNutrition - nutritionValue;
	}
	public float DecreasePoison(float amount)
	{
		float oldPoison = poisonValue;
		poisonValue = Mathf.Max(poisonValue - amount, 0f);
		return oldPoison - poisonValue;
	}

	public override bool IsFood()
    {
        return true;
    }

    public float NutritionValue { get => nutritionValue; set => nutritionValue = value; }
    public float PoisonValue { get => poisonValue; set => poisonValue = value; }
}
