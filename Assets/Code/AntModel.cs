using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntModel : EntityModel
{
    public const float ANT_SPEED = 0.01f;
    public const float TURNING_SPEED = 0.1f;
    public const float ANT_RADIUS = 0.05f;
    public const float NEW_PHEROMONE_TRESHOLD = 0.15f;

    private float homeDistanceMemory = 0.0f;

    public float HomeDistanceMemory { get => homeDistanceMemory; set => homeDistanceMemory = value; }

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
        Vector2 rotated = getPointInFront(amount);
        this.Position = Position + rotated;
    }

    private Vector2 getPointInFront(float distance)
    {
        Vector2 forwardVec = new Vector2(0f, distance);
        Vector2 rotated = Util.rotateVector(forwardVec, Rotation);
        return rotated;
    }

    private void IncrementHomeDistance(float amount)
    {
        HomeDistanceMemory += amount;
    }

    public void WalkForward()
    {
        IncrementHomeDistance(ANT_SPEED);
        MoveForward(ANT_SPEED);
        if (Carrying != null)
        {
            Carrying.Position = Position+getPointInFront(Radius+Carrying.Radius);
        }
    }

    public void RotateRandom()
    {
        int direction = Random.Range(0, 2);
        float amount = direction == 0 ? -TURNING_SPEED : TURNING_SPEED;
        Rotate(amount);
    }

    public void RandomOrientation()
    {
        this.Rotation = Random.Range(-Mathf.PI, Mathf.PI);
    }

    internal void PheromoneActions(GameState state)
    {
        PheromoneModel closest = state.findNearestPheromone(Position);
        float distance = closest == null ? float.MaxValue : (closest.Position - Position).magnitude;
        if (distance > NEW_PHEROMONE_TRESHOLD)
        {
            PheromoneModel phero = new PheromoneModel();
            phero.Position = Position;
            phero.HomeDistance = homeDistanceMemory;
            state.SpawnPheromone(phero);
        }
    }
}
