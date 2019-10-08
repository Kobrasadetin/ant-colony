using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public const int INITIAL_ANTS = 5;
    public const int INITIAL_FOOD = 16;
	public float INITIAL_REPELLANT_STRENGTH = 0.33f;
	public float INITIAL_ATTRACT_STRENGTH = 0.25f;
	public float NEW_ATTRACT_STRENGTH_TRSHLD = 0.02f;
	public const int RANDOM_PHEROMONES = 10;

	public const float REPELLANT_DIST = 0.1f;
	public const float REPELLANT_REMOVE_DIST = 0.1f;
	public const float ATTRACT_REMOVE_DIST = 0.1f;
	public const float ATTRACT_DIST = 0.08f;
	private AnthillModel playerAnthill;
    private List<AntModel> ants;
    private List<FoodModel> foods;
	private List<SourceModel> sources;
	private SpacePartitionList<PheromoneModel> pheromones;
    private int tickCounter = 0;

	public LevelSettings levelSettings;
    public AnthillModel PlayerAnthill { get => playerAnthill; set => playerAnthill = value; }
    public List<AntModel> Ants { get => ants; set => ants = value; }
    public int TickCounter { get => tickCounter; set => tickCounter = value; }
    public List<FoodModel> Foods { get => foods; set => foods = value; }
    public SpacePartitionList<PheromoneModel> Pheromones { get => pheromones; set => pheromones = value; }
	public List<SourceModel> Sources { get => sources; set => sources = value; }

	public GameState()
    {
        playerAnthill = new AnthillModel();
        ants = new List<AntModel>();
        foods = new List<FoodModel>();
		sources = new List<SourceModel>();
		Pheromones = new SpacePartitionList<PheromoneModel>();
        for (int i = 0; i < INITIAL_ANTS; i++)
        {
			addAntToNest();
        }

        for (int i = 0; i < INITIAL_FOOD; i++)
        {
			Vector2 position = Vector2.zero;
			while ((position - playerAnthill.Position).magnitude < 2.5f)
			{
				position = Random.insideUnitCircle * 12;
			}

			//Foods.Add(new FoodModel(position, 0.05f));
			sources.Add(new SourceModel(position, 1.0f, 0.0f));

		}

    }

    public void AddRepellantPheromone(Vector2 position)
    {
		float closestRepellant = float.MaxValue;	
		List<PheromoneModel> closeByPheromones = Pheromones.FindInRange(position, REPELLANT_REMOVE_DIST);
		foreach (PheromoneModel pher in closeByPheromones)
        {
			if (pher.IsRepellant)
			{
				closestRepellant = Mathf.Min(Vector2.Distance(pher.Position, position), closestRepellant);
			}
			else
			{
				pheromones.Remove(pher);
			}
        }
		if (closestRepellant > REPELLANT_DIST)
		{
			PheromoneModel newPhero = new PheromoneModel();
			newPhero.Position = position;
			newPhero.Strength = INITIAL_REPELLANT_STRENGTH;
			newPhero.IsRepellant = true;
			newPhero.HomeDistance = float.MaxValue;
			newPhero.FoodDistance = float.MaxValue;
			pheromones.Add(newPhero);
		}
    }
	public void AddAttractPheromone(Vector2 position)
	{
		float closestAttract = float.MaxValue;
		List<PheromoneModel> closeByPheromones = Pheromones.FindInRange(position, ATTRACT_REMOVE_DIST);
		foreach (PheromoneModel pher in closeByPheromones)
		{
			if (pher.IsAttract && pher.Strength > INITIAL_ATTRACT_STRENGTH - NEW_ATTRACT_STRENGTH_TRSHLD)
			{
				closestAttract = Mathf.Min(Vector2.Distance(pher.Position, position), closestAttract);
			}
			else
			{
				pheromones.Remove(pher);
			}
		}
		if (closestAttract > ATTRACT_DIST)
		{
			PheromoneModel newPhero = new PheromoneModel();
			newPhero.Position = position;
			newPhero.Strength = INITIAL_ATTRACT_STRENGTH;
			newPhero.IsAttract = true;
			newPhero.HomeDistance = float.MaxValue;
			newPhero.FoodDistance = float.MaxValue;
			pheromones.Add(newPhero);
		}
	}

	private void addAntToNest(){
		AntModel ant = new AntModel(PlayerAnthill.Position);
		ant.RandomOrientation();

		ant.MoveForward(Random.Range(0, Random.Range(0f, playerAnthill.Radius)));
		ants.Add(ant);
	}

	public int GetAntCount(){
		return Ants.Count;
	}

	internal float GetPlayerFood()
	{
		return playerAnthill.FoodStorage;
	}


	public void AddFoodSource(Vector2 position, float nutrition, float poison){
		sources.Add(new SourceModel(position, nutrition, poison));
	}

    public void update()
    {
		playerAnthill.update();
		foreach (SourceModel source in sources){
			source.update(this);
		}
		List<AntModel> deadAnts = new List<AntModel>();
        foreach (AntModel ant in ants)
        {      
            if (ant.IsInside(playerAnthill))
            {
                antHomeActions(ant);
            } else {
				pickUpFoods(ant);
			}
			ant.TickActions(this);
			if (ant.IsDead()){
				deadAnts.Add(ant);
				ant.Die();
				foods.Add(FoodModel.DeadAnt(ant));
			}
        }
		ants.RemoveAll(ant => deadAnts.Contains(ant));
		foreach (PheromoneModel phero in pheromones)
		{
			phero.update();
		}
		pheromones.RemoveAll(pheromone => pheromone.IsOld());

		sources.RemoveAll(source => source.IsExhausted());

		tickCounter++;
        
    }

	public FoodModel findNearestFood(Vector2 position)
	{
		return Search<FoodModel>.Nearest(Foods, position);
	}

	public class Search<T> where T : IObjectModel
	{
		public static List<T> ObjectsInRange(List<T> fromList, Vector2 position, float range)
		{
			List<T> list = new List<T>();
			if (fromList.Count == 0)
			{
				return list;
			}
			float treshold = range * range;
			foreach (T model in fromList)
			{
				//TODO space partition
				Vector2 v = position - model.Position;
				float d2 = (v.x * v.x + v.y * v.y);
				if (d2 < treshold)
				{
					list.Add(model);
				}
			}
			return list;
		}
		public static T Nearest(List<T> fromList, Vector2 position)
		{
			if (fromList.Count == 0)
			{
				return default(T);
			}
			T closest = default(T);
			float dist2 = float.MaxValue;
			foreach (T model in fromList)
			{
				//TODO space partition
				Vector2 v = position - model.Position;
				float d2 = (v.x * v.x + v.y * v.y);
				if (d2 < dist2)
				{
					dist2 = d2;
					closest = model;
				}
			}
			return closest;
		}

	}

	internal bool AddAnt()
	{
		if (playerAnthill.FoodStorage > 1f){
			playerAnthill.DecreaseFood(1f);
			addAntToNest();
			return true;
		}
		return false;
	}

	public List<PheromoneModel> findPheromonesInRange(Vector2 position, float range)
    {		
        return Pheromones.FindInRange(position, range);
    }

    public void SpawnPheromone(PheromoneModel pheromone)
    {
        Pheromones.Add(pheromone);
    }
	public void SpawnFood(FoodModel food)
	{
		foods.Add(food);
	}

	private void antHomeActions(AntModel ant)
    {
        ant.HomeDistanceMemory = 0f;
		ant.FoodDistanceMemory = float.MaxValue;
		ant.RemoveConfusion();
        if (ant.Carrying != null)
        {
			EntityModel dropped = ant.Carrying;
			ant.Drop();
			if (dropped.IsFood()){
				FoodModel food = (FoodModel)dropped;
				if (food.NutritionValue > food.PoisonValue){
					//good food
					playerAnthill.Happiness += 1;
					playerAnthill.IncreaseFood( food.NutritionValue );
				}
				if (food.NutritionValue < food.PoisonValue)
				{
					//bad food
					playerAnthill.Happiness -= 1;
				}
				if (food.BuildingBlockValue > 0)
				{
					//bad food
					playerAnthill.Happiness += 1;
				}
				foods.Remove(food);
			}
        }
		//eat
		float antHunger = ant.Hunger;
		ant.DecreaseHunger(playerAnthill.DecreaseFood(antHunger));

	}

    private void pickUpFoods(AntModel ant)
    {
        if (ant.Carrying == null)
        {
			FoodModel nearestFood = null;
			float foodDistance = float.MaxValue;
            foreach (FoodModel food in foods)
            {
                float dist = Vector2.Distance(ant.JawPosition(), food.Position) - food.Radius - AntModel.JAW_RADIUS;
				if (dist < foodDistance && dist < AntModel.SNIFFING_RANGE && food.CarriedBy == null)
				{
					foodDistance = dist;
					nearestFood = food;
				}
                if (dist < 0)
                {
					ant.PickUp(food);
                    break;
                }

            }
			ant.KnownNearestFood = nearestFood;
		}

    }

    public int AntCount()
    {
        return ants.Count;
    }
}
