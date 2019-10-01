using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnthillModel: BasicModel
{
    private int health;

    public int Health { get => health; set => health = value; }

    public AnthillModel() : base(0.5f)
    {

    }
}
