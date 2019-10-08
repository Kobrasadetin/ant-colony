using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodModel : EntityModel
{
    private float nutritionValue;
    private float poisonValue;
	public int BuildingBlockValue = 0;
	public const float DEFAULT_RADIUS = 0.05f;
	public Type type;
	public enum Type
	{
		NONE,
		DEAD_ANT,
		APPLE,
		PURPLE,
		RED,
		PINE,
		BONE,
		WASP,
		CUBE,
		BREAD,
		BREAD2,
		STICK,
		WASTE,
		WASTE2,
		BLUE,
		LIGHT_PURPLE,
		DARK_PURPLE,
		MUSHROOM
	}

	public static FoodModel DeadAnt(AntModel fromAnt){
		return new FoodModel(Type.DEAD_ANT, fromAnt.Position, DEFAULT_RADIUS, 1.0f - fromAnt.Hunger + 0.1f, fromAnt.Poison);
	}

	public FoodModel(Vector2 position, FoodModel other) : base(other.Radius)
	{
		this.type = other.type;
		this.nutritionValue = other.nutritionValue;
		this.poisonValue = other.poisonValue;
		this.Position = position;
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
