﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodModel : EntityModel
{
    private float nutritionValue;
    private float poisonValue;
	public const float DEFAULT_RADIUS = 0.05f;

	public FoodModel(Vector2 position, float radius) : base(radius)
    {
		this.nutritionValue = 1.0f;
		this.poisonValue = 0.0f;
		this.Position = new Vector2(position.x, position.y);
    }

    public override bool IsFood()
    {
        return true;
    }

    public float NutritionValue { get => nutritionValue; set => nutritionValue = value; }
    public float PoisonValue { get => poisonValue; set => poisonValue = value; }
}
