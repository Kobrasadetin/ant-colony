using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    private AnthillModel playerAnthill;
    private List<AntModel> ants;

    public AnthillModel PlayerAnthill { get => playerAnthill; set => playerAnthill = value; }
    public List<AntModel> Ants { get => ants; set => ants = value; }

    public GameState()
    {
        playerAnthill = new AnthillModel();
    }
}
