using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : RenderObject
{
	public SpriteRenderer feelingsRenderer;
	public GameObject mainGraphics;
	public Sprite hunger;
	public Sprite confusion;
	public Sprite starving;
	public void updateAntFeelings(AntModel model){
		mainGraphics.transform.localScale = Vector3.one;
		if (model.SlowMove()) {
			mainGraphics.transform.localScale = Vector3.one * 1.5f;
		}
		feelingsRenderer.enabled = false;
		if (model.IsConfused())
		{
			feelingsRenderer.sprite = confusion;
			feelingsRenderer.enabled = true;
		}
		if (model.IsHungry() && blinkCutoff())
		{
			feelingsRenderer.color = new Color(1f, 1f, 1f);
			feelingsRenderer.sprite = hunger;
			feelingsRenderer.enabled = true;
			if (model.IsStarving()){
				feelingsRenderer.sprite = starving;
			}
		}
	}
	public bool blinkCutoff(){
		return (Time.frameCount & 32) == 0;
	}

	public void updateAntPosition(EntityModel model)
	{
		transform.position = new Vector3(model.Position.x, model.Position.y, transform.position.z);
		mainGraphics.transform.eulerAngles = new Vector3(0, 0, model.Rotation * Mathf.Rad2Deg);
	}
}
