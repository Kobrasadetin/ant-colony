using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : RenderObject
{
	public SpriteRenderer confusion;
	public GameObject mainGraphics;
    public void updateAntFeelings(AntModel model){
		confusion.color = new Color(1.0f, 1.0f, 1.0f, model.Confusion > AntModel.ANT_CONFUSED_TRESHOLD ? 1.0f: 0f);
	}
	public void updateAntPosition(EntityModel model)
	{
		transform.position = new Vector3(model.Position.x, model.Position.y, transform.position.z);
		mainGraphics.transform.eulerAngles = new Vector3(0, 0, model.Rotation * Mathf.Rad2Deg);
	}
}
