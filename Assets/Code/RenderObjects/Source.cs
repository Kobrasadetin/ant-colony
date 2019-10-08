using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : RenderObject
{
	private SpriteRenderer spriteRenderer;
	public GameObject[] sourcePrefabs;
	public override void initializeState(EntityModel model)
	{
		this.spriteRenderer = GetComponent<SpriteRenderer>();
		base.initializeState(model);
		SourceModel source = (SourceModel)model;
		Instantiate<GameObject>(sourcePrefabs[(int)source.FoodModelProto.type], transform);
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
