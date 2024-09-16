using UnityEngine;

public interface IdleMovementInterface
{
    public abstract void StopMovement();
    public abstract void StartMovement();
    public abstract float GetSpeed();
}
