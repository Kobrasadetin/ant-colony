using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class RenderObject : MonoBehaviour
{
	public void remove()
	{
		Destroy(gameObject);
	}

	public virtual void initializeState(EntityModel model)
	{
		updateState(model);
	}
	public virtual void updateState(EntityModel model)
    {
        this.transform.position = new Vector3(model.Position.x, model.Position.y, transform.position.z);
        this.transform.eulerAngles = new Vector3( 0, 0 , model.Rotation * Mathf.Rad2Deg);
    }
    public void updatePositionBasic(BasicModel model)
    {
        this.transform.position = new Vector3(model.Position.x, model.Position.y, transform.position.z);
    }
}
