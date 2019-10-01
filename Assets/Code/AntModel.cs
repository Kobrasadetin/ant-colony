using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntModel : BasicModel
{
    public const float ANT_SPEED = 0.01f;
    public const float TURNING_SPEED = 0.1f;


    public AntModel(Vector2 position) : base(0.05f)
    {
        this.Position = position;
    }

    public void Rotate(float amount)
    {
        this.Rotation = Rotation + amount;
    }

    public void MoveForward()
    {
        Vector2 forwardVec = new Vector2(0f, ANT_SPEED);
        Vector2 rotated = Util.rotateVector(forwardVec, Rotation);
        this.Position = Position + rotated;
    }

    public void RotateRandom()
    {
        int direction = Random.Range(0, 2);
        float amount = direction == 0 ? -TURNING_SPEED : TURNING_SPEED;
        Rotate(amount);
    }
}
