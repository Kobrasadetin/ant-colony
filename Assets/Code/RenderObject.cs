using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class RenderObject : MonoBehaviour
{
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public void updatePosition(EntityModel model)
    {
        this.transform.position = new Vector3(model.Position.x, model.Position.y, transform.position.z);
        this.transform.eulerAngles = new Vector3( 0, 0 , model.Rotation * Mathf.Rad2Deg);
    }
    public void updatePositionBasic(BasicModel model)
    {
        this.transform.position = new Vector3(model.Position.x, model.Position.y, transform.position.z);
    }
}
