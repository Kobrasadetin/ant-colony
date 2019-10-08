using System.Collections.Generic;
using UnityEngine;

public partial class AntModel : EntityModel
{
	public const float ANT_SPEED = 0.01f;
	public const float TURNING_SPEED = 0.1f;
	public const float ANT_RADIUS = 0.04f;
	public const float JAW_RADIUS = 0.01f;
	public const float NEW_PHEROMONE_TRESHOLD = 0.15f;
	public const float SNIFFING_RANGE = 0.3f;
	public const float SLOWDOWN_TRESHOLD = 0.15f;
	public const float FOOD_SPREAD_MULTIPLIER = 1.5f;
	public const float ANT_CONFUSED_TRESHOLD = 0.7f;
	public const float PHERO_CONFUSE_TRSHLD = 0.3f;

	private float foodDistanceMemory = float.MaxValue;
	private float homeDistanceMemory = 0.0f;
	private float confusion = 0.0f;
	private Mission mission;
	private FoodModel knownNearestFood;
	private float hunger = 0f;
	private float poison = 0f;
	private float health = 1f;
	private bool foodDisappointment = false;
	private bool homeDisappointment = false;
	private bool shouldSlowDown = false;
	private float missionProgress = float.MaxValue;
	private BasicModel walkingTarget;
	private BasicModel avoidTarget;

	public float HomeDistanceMemory { get => homeDistanceMemory; set => homeDistanceMemory = value; }
	public float FoodDistanceMemory { get => foodDistanceMemory; set => foodDistanceMemory = value; }
	public float Confusion
	{
		get => confusion;
		set => confusion = Mathf.Clamp(value, 0, 1);
	}
	public FoodModel KnownNearestFood { get => knownNearestFood; set => knownNearestFood = value; }
	public float Hunger => hunger;
	public float Health { get => health; set => health = value; }
	public float Poison { get => poison; }

	public void IncreaseHunger(float amount)
	{
		hunger = Mathf.Min(hunger + amount, 1f);
	}
	public void IncreasePoison(float amount)
	{
		poison += amount;
	}
	public void DecreasePoison(float amount)
	{
		poison = Mathf.Max(poison - amount, 0f);
	}
	public float DecreaseHunger(float amount)
	{
		float oldHunger = hunger;
		hunger = Mathf.Max(hunger - amount, 0f);
		return oldHunger - hunger;
	}
	public float DecreaseHealth(float amount)
	{
		float oldHealth = health;
		health = Mathf.Max(health - amount, 0f);
		return oldHealth - health;
	}
	public bool IsHungry()
	{
		return hunger > 0.7f;
	}
	public bool IsStarving()
	{
		return hunger > 0.99f;
	}
	public bool IsDead()
	{
		return health <= 0f;
	}
	public void Die()
	{
		if (Carrying != null)
		{
			Carrying.CarriedBy = null;
			Carrying = null;
		}
	}
	public bool IsConfused()
	{
		return Confusion > ANT_CONFUSED_TRESHOLD;
	}

	public enum MissionType
	{
		GO_HOME,
		FORAGE,
		FOOD_OR_HOME,
	}

	public void TickActions(GameState gameState) {
		DecideMission();
		PheromoneActions(gameState);
		DecideRotation();
		WalkForward();
		AdvanceTime();
	}

	public void UpdateMission(Mission mission)
	{
		if (mission != this.mission)
		{
			this.mission = mission;
			float lastProgress = float.MaxValue;
			if (mission.GoHome)
			{
				lastProgress = Mathf.Min(homeDistanceMemory, lastProgress);
			}

			if (mission.SearchFood)
			{
				lastProgress = Mathf.Min(foodDistanceMemory, lastProgress);
			}
		}
	}
	private void EvaluateMissionProgress()
	{
		if (mission.type == MissionType.GO_HOME)
		{
			//we get confused if home seems further away
			float newConfusion = Mathf.Max(homeDistanceMemory - missionProgress + 0.06f, -0.05f);
			if (newConfusion > 0f)
			{
				homeDisappointment = true;
			}
			missionProgress = homeDistanceMemory;
			Confusion += newConfusion;
		}
		if (mission.type == MissionType.FOOD_OR_HOME)
		{
			if (knownNearestFood == null && Carrying == null)
			{
				float deltaConfusion = Mathf.Clamp(Mathf.Min(foodDistanceMemory, homeDistanceMemory) - missionProgress + 0.05f, -0.05f, 0.5f);
				if (deltaConfusion > 0f)
				{
					foodDisappointment = true;
				}
				missionProgress = Mathf.Min(foodDistanceMemory, homeDistanceMemory);
				Confusion += deltaConfusion;
			}
		}
		if (mission.type == MissionType.FORAGE)
		{
			if (knownNearestFood == null && Carrying == null)
			{
				float deltaConfusion = Mathf.Clamp(foodDistanceMemory - missionProgress + 0.05f, -0.05f, 0.5f);
				if (deltaConfusion > 0f)
				{
					foodDisappointment = true;
				}
				missionProgress = foodDistanceMemory;
				Confusion += deltaConfusion;
			}
		}
	}

	public bool SlowMove()
	{
		return shouldSlowDown;
	}

	public void RemoveConfusion()
	{
		confusion = 0f;
	}

	public AntModel(Vector2 position) : base(ANT_RADIUS)
	{
		Position = position;
		UpdateMission(ToForage);
	}

	public void Rotate(float amount)
	{
		Rotation = Rotation + amount;
	}

	public void MoveForward(float amount)
	{
		Vector2 rotated = ForwardVector(amount);
		Position = Position + rotated;
	}

	public Vector2 ForwardVector(float distance)
	{
		Vector2 forwardVec = new Vector2(0f, distance);
		Vector2 rotated = Util.rotateVector(forwardVec, Rotation);
		return rotated;
	}
	public Vector2 JawPosition()
	{
		return Position + ForwardVector(Radius);
	}

	private void IncrementMemory(float amount)
	{
		HomeDistanceMemory += amount;
		FoodDistanceMemory += amount;
	}

	public void AdvanceTime()
	{
		IncreaseHunger(0.0002f);
		if (Carrying != null && Carrying.IsFood() && hunger > 0.1f)
		{
			FoodModel food = (FoodModel)Carrying;
			DecreaseHunger(food.DecreaseNutrition(0.05f));
			IncreasePoison(food.DecreasePoison(0.05f));
		}
		if (IsStarving())
		{
			DecreaseHealth(0.001f);
		}
		DecreaseHealth(poison * 0.001f);
		DecreasePoison(0.001f);
	}

	public void WalkForward()
	{
		Confusion -= 0.005f;
		IncrementMemory(ANT_SPEED);
		MoveForward(shouldSlowDown ? ANT_SPEED * 0.5f : ANT_SPEED);
		if (Carrying != null)
		{
			Carrying.Position = Position + ForwardVector(Radius + Carrying.Radius);
		}
	}

	private float RandomRotation()
	{
		int direction = Random.Range(0, 2);
		return direction == 0 ? -TURNING_SPEED : TURNING_SPEED;
	}

	public void DecideRotation()
	{
		if (avoidTarget!=null){
			float dot = CalculateHeading(avoidTarget.Position);
			float dir = TURNING_SPEED;
			if (dot < 0.0f)
			{
				dir = -TURNING_SPEED;
			}
			Rotate(dir);
			return;
		}
		if (Confusion > ANT_CONFUSED_TRESHOLD)
		{
			walkingTarget = null;
			Rotate(RandomRotation());
			return;
		}
		if (knownNearestFood != null && knownNearestFood.CarriedBy == null && mission.SearchFood)
		{
			walkingTarget = knownNearestFood;
		}
		else if (Random.Range(0, 2) == 0 || Confusion > ANT_CONFUSED_TRESHOLD)
		{
			Rotate(RandomRotation());
			return;
		}

		if (walkingTarget != null && IsInside(walkingTarget))
		{
			//we reached our walking target, should we be more confused?
			EvaluateMissionProgress();
			walkingTarget = null;
		}
		if (walkingTarget == null)
		{
			Rotate(RandomRotation());
		}
		else
		{
			float dot = CalculateHeading(walkingTarget.Position);

			//shouldSlowDown = toTarget.magnitude < SLOWDOWN_TRESHOLD && Mathf.Abs(dot) > 0.6;

			float dir = TURNING_SPEED;

			// turn left
			if (dot > 0.0f)
			{
				dir = -TURNING_SPEED;
			}
			Rotate(dir);
		}
	}

	private float CalculateHeading(Vector2 target)
	{
		Vector2 toTarget = target - Position;
		Vector2 perpendicular = Util.rotateVector(toTarget.normalized, Mathf.PI / 2);
		Vector2 heading = ForwardVector(1f);
		float dot = Vector2.Dot(perpendicular, heading);
		return dot;
	}

	public void RandomOrientation()
	{
		Rotation = Random.Range(-Mathf.PI, Mathf.PI);
	}

	public void DecideMission()
	{
		if (Carrying != null && Carrying.IsFood())
		{
			UpdateMission(ToGoHome);
		}
		else
		{
			UpdateMission(ToForage);
		}
		if (IsHungry())
		{
			UpdateMission(ToStopStarving);
		}
	}

	public void PickUp(EntityModel entity)
	{
		if (entity.IsFood() && entity.CarriedBy == null)
		{
			RemoveConfusion();
			foodDistanceMemory = 0.0f;
			entity.CarriedBy = this;
			Carrying = entity;
		}
	}
	public void Drop()
	{
		Carrying.CarriedBy = null;
		Carrying = null;
	}

	internal void PheromoneActions(GameState state)
	{
		walkingTarget = null;
		List<PheromoneModel> closebyPheromones = state.findPheromonesInRange(Position, SNIFFING_RANGE);
		PheromoneAreaInfo info = FindInfoPheromone(closebyPheromones);
		float distance = info.closest == null ? float.MaxValue : (info.closest.Position - Position).magnitude;
		var closest = info.closest;
		if (closest != null && IsInside(closest) && !closest.IsRepellant)
		{
			closest.Strength = 1.0f;
			if (Confusion < 0.1f)
			{
				closest.Confusion = 0f;
			}
			else
			{
				//Debug.Log("confusion!");
			}
			closest.Confusion = (9 * closest.Confusion + Confusion) * (1 / 10f);
			Confusion = Mathf.Max(closest.Confusion, Confusion);

			if (closest.Confusion < PHERO_CONFUSE_TRSHLD)
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
					//foodDistanceMemory = closest.FoodDistance;
				}
				else
				{
					closest.FoodDistance = Mathf.Min(foodDistanceMemory, closest.FoodDistance);
				}
			}
			if (closest.IsAttract)
			{
				closest.IsAttract = false;
				homeDisappointment = false;
				foodDisappointment = false;
			}
			else
			{
				if (homeDisappointment)
				{
					closest.HomeDistance = homeDistanceMemory;
					homeDisappointment = false;
				}
				if (foodDisappointment)
				{
					closest.FoodDistance = float.MaxValue;
					foodDisappointment = false;
				}
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
			avoidTarget = info.repel;
			float bestTargetScore = float.MinValue;
			if (info.attract != null)
			{
				walkingTarget = info.attract;
			}
			else
			{
				foreach (PheromoneModel phero in closebyPheromones)
				{
					float targetScore = mission.evaluatePhero(phero);
					if (targetScore > bestTargetScore)
					{
						bestTargetScore = targetScore;
						walkingTarget = phero;
					}
				}
			}
		}

	}
	private PheromoneModel AvoidConfusing(PheromoneModel phero)
	{
		if (phero == null || phero.Confusion > PHERO_CONFUSE_TRSHLD)
		{
			return null;
		}
		else
		{
			return phero;
		}
	}

	private struct PheromoneAreaInfo {	
		public PheromoneModel closest;
		public PheromoneModel home;
		public PheromoneModel food;
		public PheromoneModel repel;
		public PheromoneModel attract;
	}

	private PheromoneAreaInfo FindInfoPheromone(List<PheromoneModel> closebyPheromones)
	{
		PheromoneAreaInfo info = new PheromoneAreaInfo();
		float strongestAttract = float.MinValue;
		float closestRepel = float.MaxValue;
		float homeSmell = float.MaxValue;
		float foodSmell = float.MaxValue;
		float dist2 = float.MaxValue;
		foreach (PheromoneModel phero in closebyPheromones)
		{
			Vector2 v = Position - phero.Position;
			float d2 = (v.x * v.x + v.y * v.y);
			if (d2 < dist2)
			{
				dist2 = d2;
				info.closest = phero;
			}
			if (phero.HomeDistance < homeSmell)
			{
				homeSmell = phero.HomeDistance;
				info.home = phero;
			}
			if (phero.FoodDistance < foodSmell)
			{
				foodSmell = phero.FoodDistance;
				info.food = phero;
			}
			if (phero.IsRepellant && d2 < closestRepel)
			{
				closestRepel = d2;
				info.repel = phero;
			}
			if (phero.IsAttract && phero.Strength > strongestAttract)
			{
				strongestAttract = phero.Strength;
				info.attract = phero;
			}
		}
		return info;
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
