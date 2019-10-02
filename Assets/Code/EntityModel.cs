using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModel:BasicModel
{
    private float rotation;
    private EntityModel carriedBy = null;
    private EntityModel carrying = null;

    public virtual bool IsFood()
    {
        return false;
    }

    public float Rotation { get => rotation; set => rotation = value; }
    public EntityModel CarriedBy
    {
        get => carriedBy; set
        {
            if (CarriedBy != null)
            {
                CarriedBy.Carrying = null;
            }
            carriedBy = value;
        }
    }

    public EntityModel Carrying { get => carrying; protected set => carrying = value; }

    public EntityModel(float radius)
    {
        this.Radius = radius;
    }

}
