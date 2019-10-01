using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodModel : EntityModel
{
    private float nutritionValue;
    private float poisonValue;
    public FoodModel(Vector2 position, float radius) : base(radius)
    {
        this.Position = position;
    }

    public float NutritionValue { get => nutritionValue; set => nutritionValue = value; }
    public float PoisonValue { get => poisonValue; set => poisonValue = value; }
}
