using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodModel : BasicModel
{
    private float nutritionValue;
    private float poisonValue;
    public FoodModel(float radius) : base(radius)
    {

    }

    public float NutritionValue { get => nutritionValue; set => nutritionValue = value; }
    public float PoisonValue { get => poisonValue; set => poisonValue = value; }
}
