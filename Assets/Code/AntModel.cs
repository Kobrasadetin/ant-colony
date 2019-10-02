using System.Collections.Generic;
using UnityEngine;

public class AntModel : EntityModel
{
	public const float ANT_SPEED = 0.01f;
	public const float TURNING_SPEED = 0.1f;
	public const float ANT_RADIUS = 0.05f;
	public const float NEW_PHEROMONE_TRESHOLD = 0.15f;
	public const float SNIFFING_RANGE = 0.3f;
	public const float FOOD_SPREAD_MULTIPLIER = 1.5f;

	private float foodDistanceMemory = float.MaxValue;
	private float homeDistanceMemory = 0.0f;
	private BasicModel walkingTarget = null;

	public float HomeDistanceMemory { get => homeDistanceMemory; set => homeDistanceMemory = value; }
	public BasicModel WalkingTarget { get => walkingTarget; set => walkingTarget = value; }
	public float FoodDistanceMemory { get => foodDistanceMemory; set => foodDistanceMemory = value; }

	public AntModel(Vector2 position) : base(ANT_RADIUS)
	{
		Position = position;
	}

	public void Rotate(float amount)
	{
		Rotation = Rotation + amount;
	}

	public void MoveForward(float amount)
	{
		Vector2 rotated = GetOrientationVector(amount);
		Position = Position + rotated;
	}

	private Vector2 GetOrientationVector(float distance)
	{
		Vector2 forwardVec = new Vector2(0f, distance);
		Vector2 rotated = Util.rotateVector(forwardVec, Rotation);
		return rotated;
	}

	private void IncrementMemory(float amount)
	{
		HomeDistanceMemory += amount;
		FoodDistanceMemory += amount;
	}

	public void WalkForward()
	{
		IncrementMemory(ANT_SPEED);
		MoveForward(ANT_SPEED);
		if (Carrying != null)
		{
			Carrying.Position = Position + GetOrientationVector(Radius + Carrying.Radius);
		}
	}

	private float RandomRotation()
	{
		int direction = Random.Range(0, 2);
		return direction == 0 ? -TURNING_SPEED : TURNING_SPEED;
	}

	public void DecideRotation()
	{
		if (walkingTarget != null && IsInside(walkingTarget))
		{
			walkingTarget = null;
		}
		if (WalkingTarget == null)
		{
			Rotate(RandomRotation());
		}
		else
		{
			Vector2 target = Util.rotateVector(WalkingTarget.Position - Position, Mathf.PI / 2);
			Vector2 heading = GetOrientationVector(1f);
			float dot = Vector2.Dot(target, heading);

			float dir = TURNING_SPEED;

			// turn left
			if (dot > 0.0f)
			{
				dir = -TURNING_SPEED;
			}
			Rotate(dir);
		}
	}

	public void RandomOrientation()
	{
		Rotation = Random.Range(-Mathf.PI, Mathf.PI);
	}

	public void PickUp(EntityModel entity)
	{
		if (entity.IsFood() && entity.CarriedBy == null)
		{
			foodDistanceMemory = 0.0f;
		}
		entity.CarriedBy = this;
		Carrying = entity;
	}
	public void Drop()
	{
		Carrying.CarriedBy = null;
		Carrying = null;
	}

	internal void PheromoneActions(GameState state)
	{
		List<PheromoneModel> closebyPheromones = state.findPheromonesInRange(Position, SNIFFING_RANGE);
		PheromoneModel closest;
		PheromoneModel homeDirection;
		PheromoneModel foodDirection;
		FindInfoPheromone(closebyPheromones, out closest, out homeDirection, out foodDirection);
		float distance = closest == null ? float.MaxValue : (closest.Position - Position).magnitude;
		if (closest != null && IsInside(closest))
		{
			if (closest.HomeDistance < homeDistanceMemory)
			{
				homeDistanceMemory = closest.HomeDistance;
			}
			else
			{
				closest.HomeDistance = Mathf.Min(homeDistanceMemory, closest.HomeDistance);				
			}
			if (closest.FoodDistance < foodDistanceMemory)
			{
				//closest.FoodDistance += 0.01f;
			}
			else
			{
				closest.FoodDistance = Mathf.Min(foodDistanceMemory, closest.FoodDistance);
			}
		}
		if (distance > NEW_PHEROMONE_TRESHOLD)
		{
			PheromoneModel phero = new PheromoneModel
			{
				Position = Position,
				HomeDistance = homeDistanceMemory,
				FoodDistance = foodDistanceMemory
			};
			state.SpawnPheromone(phero);
		}
		if (closest != null)
		{
			if (Carrying != null && Carrying.IsFood())
			{
				walkingTarget = homeDirection;
			}
			if (Carrying == null)
			{
				//if (foodDirection != null && foodDirection.FoodDistance < 2f)
				walkingTarget = foodDirection;
			}
		}
       
	}

	private void FindInfoPheromone(List<PheromoneModel> closebyPheromones, out PheromoneModel closest, out PheromoneModel home, out PheromoneModel food)
	{
		float homeSmell = float.MaxValue;
		float foodSmell = float.MaxValue;
		float dist2 = float.MaxValue;
		closest = null;
		home = null;
		food = null;
		foreach (PheromoneModel phero in closebyPheromones)
		{
			Vector2 v = Position - phero.Position;
			float d2 = (v.x * v.x + v.y * v.y);
			if (d2 < dist2)
			{
				dist2 = d2;
				closest = phero;
			}
			if (phero.HomeDistance < homeSmell)
			{
				homeSmell = phero.HomeDistance;
				home = phero;
			}
			if (phero.FoodDistance < foodSmell)
			{
				foodSmell = phero.FoodDistance;
				food = phero;
			}
		}
	}
	private PheromoneModel FindNearestPheromone(List<PheromoneModel> closebyPheromones)
	{
		if (closebyPheromones.Count == 0)
		{
			return null;
		}
		PheromoneModel closest = null;
		float dist2 = float.MaxValue;
		foreach (PheromoneModel phero in closebyPheromones)
		{
			//TODO space partition
			Vector2 v = Position - phero.Position;
			float d2 = (v.x * v.x + v.y * v.y);
			if (d2 < dist2)
			{
				dist2 = d2;
				closest = phero;
			}
		}
		return closest;
	}
}
