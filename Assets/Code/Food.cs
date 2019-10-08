using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : RenderObject
{
	public GameObject deadAnt;
	public Sprite[] foodSprites;
	private SpriteRenderer spriteRenderer;
	public override void initializeState(EntityModel model)
	{
		this.spriteRenderer = GetComponent<SpriteRenderer>();
		base.initializeState(model);
		FoodModel food = (FoodModel)model;
		if (food.type == FoodModel.Type.DEAD_ANT){
			Instantiate(deadAnt, this.transform);
		} else {
			spriteRenderer.sprite = foodSprites[(int)food.type];
		}
	}
}
