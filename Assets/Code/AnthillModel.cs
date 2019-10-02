using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnthillModel: EntityModel
{
    private int health = 3;
	private int healthChanged;

    public int Health { get => health; set { HealthChanged = value-health;  health = value; } }

	public int HealthChanged { get => healthChanged; private set => healthChanged = value; }

	public AnthillModel() : base(0.5f)
    {

    }
	public void update() {
		HealthChanged = 0;
	}
	

}
