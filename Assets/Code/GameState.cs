using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public const int INITIAL_ANTS = 10;
    private AnthillModel playerAnthill;
    private List<AntModel> ants;
    private int tickCounter = 0;

    public AnthillModel PlayerAnthill { get => playerAnthill; set => playerAnthill = value; }
    public List<AntModel> Ants { get => ants; set => ants = value; }
    public int TickCounter { get => tickCounter; set => tickCounter = value; }

    public GameState()
    {
        playerAnthill = new AnthillModel();
        ants = new List<AntModel>();
        for (int i = 0; i< INITIAL_ANTS; i++)
        {
            AntModel ant = new AntModel(PlayerAnthill.Position);
            ant.RandomOrientation();
  
            ant.MoveForward(Random.Range(0, Random.Range(0f, playerAnthill.Radius)));
            ants.Add(ant);
        }
        
    }

    public void update()
    {
        foreach (AntModel ant in ants)
        {
            ant.RotateRandom();
            ant.MoveForward();
        }
        tickCounter++;
    }

    public int AntCount()
    {
        return ants.Count;
    }
}
