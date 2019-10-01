using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntModel : EntityModel
{
    public const float ANT_SPEED = 0.01f;
    public const float TURNING_SPEED = 0.1f;
    public const float ANT_RADIUS = 0.05f;
    public const float NEW_PHEROMONE_TRESHOLD = 0.15f;
    public const float SNIFFING_RANGE = 0.3f;

    private float foodDistanceMemory = float.MaxValue;
    private float homeDistanceMemory = 0.0f;
    private BasicModel walkingTarget = null;

    public float HomeDistanceMemory { get => homeDistanceMemory; set => homeDistanceMemory = value; }
    public BasicModel WalkingTarget { get => walkingTarget; set => walkingTarget = value; }
    public float FoodDistanceMemory { get => foodDistanceMemory; set => foodDistanceMemory = value; }

    public AntModel(Vector2 position) : base(ANT_RADIUS)
    {
        this.Position = position;
    }

    public void Rotate(float amount)
    {
        this.Rotation = Rotation + amount;
    }

    public void MoveForward(float amount)
    {
        Vector2 rotated = GetOrientationVector(amount);
        this.Position = Position + rotated;
    }

    private Vector2 GetOrientationVector(float distance)
    {
        Vector2 forwardVec = new Vector2(0f, distance);
        Vector2 rotated = Util.rotateVector(forwardVec, Rotation);
        return rotated;
    }

    private void IncrementMemory(float amount)
    {
        HomeDistanceMemory += amount;
        FoodDistanceMemory += amount;
    }

    public void WalkForward()
    {
        IncrementMemory(ANT_SPEED);
        MoveForward(ANT_SPEED);
        if (Carrying != null)
        {
            Carrying.Position = Position+GetOrientationVector(Radius+Carrying.Radius);
        }
    }

    private float RandomRotation()
    {
        int direction = Random.Range(0, 2);
        return direction == 0 ? -TURNING_SPEED : TURNING_SPEED;
    }

    public void DecideRotation()
    {
        if (walkingTarget!= null && IsInside(walkingTarget))
        {
            walkingTarget = null;
        }
        if (WalkingTarget == null)
        {
            Rotate(RandomRotation());
        } else
        {
            Vector2 target = Util.rotateVector(WalkingTarget.Position - Position, Mathf.PI/2);
            Vector2 heading = GetOrientationVector(1f);
            float dot = Vector2.Dot(target, heading);

            float dir = TURNING_SPEED;
            
            // turn left
            if (dot > 0.0f)
            {
                dir = -TURNING_SPEED;
            }
            Rotate(dir);
        }
    }

    public void RandomOrientation()
    {
        this.Rotation = Random.Range(-Mathf.PI, Mathf.PI);
    }

    public void PickUp(EntityModel entity)
    {
        if (entity.IsFood())
        {
            foodDistanceMemory = 0.0f;
        }
    }

    internal void PheromoneActions(GameState state)
    {
        List<PheromoneModel> closebyPheromones = state.findPheromonesInRange(Position, SNIFFING_RANGE);
        PheromoneModel closest = FindNearestPheromone(closebyPheromones);
        float distance = closest == null ? float.MaxValue : (closest.Position - Position).magnitude;
        if (closest != null && IsInside(closest))
        {
            if (closest.HomeDistance < homeDistanceMemory)
            {
                homeDistanceMemory = closest.HomeDistance;
            }
            else
            {
                closest.HomeDistance = Mathf.Min(homeDistanceMemory, closest.HomeDistance);
                closest.FoodDistance = Mathf.Min(foodDistanceMemory, closest.FoodDistance);
            }
        }
        if (distance > NEW_PHEROMONE_TRESHOLD)
        {
            PheromoneModel phero = new PheromoneModel();
            phero.Position = Position;
            phero.HomeDistance = homeDistanceMemory;
            phero.FoodDistance = foodDistanceMemory;
            state.SpawnPheromone(phero);
        }
        if (closest!= null &&  Carrying != null && Carrying.IsFood())
        {
            walkingTarget = FindHomePheromone(closebyPheromones);
            
        }
    }

    private PheromoneModel FindHomePheromone(List<PheromoneModel> closebyPheromones)
    {
        PheromoneModel closest = null;
        float homeSmell = float.MaxValue;
        foreach (PheromoneModel phero in closebyPheromones)
        {
            if (phero.HomeDistance < homeSmell)
            {
                homeSmell = phero.HomeDistance;
                closest = phero;
            }
        }
        return closest;
    }
    private PheromoneModel FindNearestPheromone(List<PheromoneModel> closebyPheromones)
    {
        if (closebyPheromones.Count == 0)
        {
            return null;
        }
        PheromoneModel closest = null;
        float dist2 = float.MaxValue;
        foreach (PheromoneModel phero in closebyPheromones)
        {
            //TODO space partition
            Vector2 v = Position - phero.Position;
            float d2 = (v.x * v.x + v.y * v.y);
            if (d2 < dist2)
            {
                dist2 = d2;
                closest = phero;
            }
        }
        return closest;
    }
}
