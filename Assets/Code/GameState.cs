using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public const int INITIAL_ANTS = 16;
    public const int INITIAL_FOOD = 50;
    public const int RANDOM_PHEROMONES = 10;
    private AnthillModel playerAnthill;
    private List<AntModel> ants;
    private List<FoodModel> foods;
    private List<PheromoneModel> pheromones;
    private int tickCounter = 0;

    public AnthillModel PlayerAnthill { get => playerAnthill; set => playerAnthill = value; }
    public List<AntModel> Ants { get => ants; set => ants = value; }
    public int TickCounter { get => tickCounter; set => tickCounter = value; }
    public List<FoodModel> Foods { get => foods; set => foods = value; }

    public GameState()
    {
        playerAnthill = new AnthillModel();
        ants = new List<AntModel>();
        foods = new List<FoodModel>();
        for (int i = 0; i < INITIAL_ANTS; i++)
        {
            AntModel ant = new AntModel(PlayerAnthill.Position);
            ant.RandomOrientation();

            ant.MoveForward(Random.Range(0, Random.Range(0f, playerAnthill.Radius)));
            ants.Add(ant);
        }

        for (int i = 0; i < INITIAL_FOOD; i++)
        {
            Vector2 position = Random.insideUnitCircle * 5;
            Foods.Add(new FoodModel(position, 0.05f));
        }

    }

    public void update()
    {
        foreach (AntModel ant in ants)
        {
            pickUpFoods(ant);
            ant.RotateRandom();
            ant.MoveForward();
        }
        tickCounter++;
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
                    ant.Carrying = food;
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
