using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public const int INITIAL_ANTS = 20;
    public const int INITIAL_FOOD = 0;
    public const int RANDOM_PHEROMONES = 10;
    private AnthillModel playerAnthill;
    private List<AntModel> ants;
    private List<FoodModel> foods;
	private List<SourceModel> sources;
	private List<PheromoneModel> pheromones;
    private int tickCounter = 0;

    public AnthillModel PlayerAnthill { get => playerAnthill; set => playerAnthill = value; }
    public List<AntModel> Ants { get => ants; set => ants = value; }
    public int TickCounter { get => tickCounter; set => tickCounter = value; }
    public List<FoodModel> Foods { get => foods; set => foods = value; }
    public List<PheromoneModel> Pheromones { get => pheromones; set => pheromones = value; }

    public GameState()
    {
        playerAnthill = new AnthillModel();
        ants = new List<AntModel>();
        foods = new List<FoodModel>();
		sources = new List<SourceModel>();
		Pheromones = new List<PheromoneModel>();
        for (int i = 0; i < INITIAL_ANTS; i++)
        {
            AntModel ant = new AntModel(PlayerAnthill.Position);
            ant.RandomOrientation();

            ant.MoveForward(Random.Range(0, Random.Range( 0f, playerAnthill.Radius)));
            ant.WalkingTarget = playerAnthill;
            ants.Add(ant);
        }

        for (int i = 0; i < INITIAL_FOOD; i++)
        {
			Vector2 position = Vector2.zero;
			while ((position - playerAnthill.Position).magnitude < 1.5f)
			{
				position = Random.insideUnitCircle * 5;
			}

			//Foods.Add(new FoodModel(position, 0.05f));
			sources.Add(new SourceModel(position, 1.0f, 0.0f));

		}

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
        foreach (AntModel ant in ants)
        {      
            if (ant.IsInside(playerAnthill))
            {
                antHomeActions(ant);
            } else {
				pickUpFoods(ant);
			}
            ant.DecideRotation();
            ant.WalkForward();
            ant.PheromoneActions(this);
        }
		foreach (PheromoneModel phero in pheromones)
		{
			//phero.update();
		}
		tickCounter++;
        
    }

    public PheromoneModel findNearestPheromone(Vector2 position)
    {
        return Search< PheromoneModel>.Nearest(Pheromones, position);
    }
	public FoodModel findNearestFood(Vector2 position)
	{
		return Search<FoodModel>.Nearest(Foods, position);
	}

	private class Search<T> where T : IObjectModel
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


	public List<PheromoneModel> findPheromonesInRange(Vector2 position, float range)
    {		
        return Search<PheromoneModel>.ObjectsInRange(Pheromones, position, range);
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
        if (ant.Carrying != null)
        {
			EntityModel dropped = ant.Carrying;
			ant.Drop();
			if (dropped.IsFood()){
				FoodModel food = (FoodModel)dropped;
				if (food.NutritionValue > food.PoisonValue){
					//good food
					playerAnthill.Health += 1;
				}
				if (food.NutritionValue < food.PoisonValue)
				{
					//bad food
					playerAnthill.Health -= 1;
				}
				foods.Remove(food);
			}
            //TODO deal with dropped food
        }
    }

    private void pickUpFoods(AntModel ant)
    {
        if (ant.Carrying == null)
        {
            foreach (FoodModel food in foods)
            {
                float dist = Vector2.Distance(ant.Position, food.Position) - ant.Radius - food.Radius;
                if (dist < 0)
                {
					ant.PickUp(food);
                    break;
                }

            }
        }
    }

    public int AntCount()
    {
        return ants.Count;
    }
}
