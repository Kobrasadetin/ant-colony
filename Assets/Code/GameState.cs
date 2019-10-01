using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public const int INITIAL_ANTS = 10;
    private AnthillModel playerAnthill;
    private List<AntModel> ants;

    public AnthillModel PlayerAnthill { get => playerAnthill; set => playerAnthill = value; }
    public List<AntModel> Ants { get => ants; set => ants = value; }

    public GameState()
    {
        playerAnthill = new AnthillModel();
        ants = new List<AntModel>();
        for (int i = 0; i< INITIAL_ANTS; i++)
        {
            ants.Add(new AntModel(PlayerAnthill.Position));
        }
        
    }

    public void update()
    {
        foreach (AntModel ant in ants)
        {
            ant.RotateRandom();
            ant.MoveForward();
        }
    }
}
