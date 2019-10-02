using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceModel : BasicModel
{
	public const float BASE_RADIUS = 0.1f;
	public const float SPAWN_RADIUS = 0.1f;
	public float nutrition;
	public float poison;
    public SourceModel(Vector2 position, float nutrition, float poison)
	{
		Position = position;
		Radius = BASE_RADIUS;
		this.nutrition = nutrition;
		this.poison = poison;
	}
	public void update(GameState state){
		FoodModel nearest = state.findNearestFood(Position);
		if ((nearest == null || (nearest.Position - Position).magnitude > SPAWN_RADIUS)){
			FoodModel food = new FoodModel(Position, FoodModel.DEFAULT_RADIUS);
			food.NutritionValue = nutrition;
			food.PoisonValue = poison;
			state.SpawnFood(food);
		}
	}
}
