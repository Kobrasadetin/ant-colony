using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceModel : BasicModel
{
	public const float BASE_RADIUS = 0.1f;
	public const float SPAWN_RADIUS = 0.1f;
    public SourceModel(Vector2 position){
		Position = position;
		Radius = BASE_RADIUS;
	}
	public void update(GameState state){
		FoodModel nearest = state.findNearestFood(Position);
		if ((nearest == null || (nearest.Position - Position).magnitude > SPAWN_RADIUS)){
			state.SpawnFood(new FoodModel(Position, FoodModel.DEFAULT_RADIUS));
		}
	}
}
