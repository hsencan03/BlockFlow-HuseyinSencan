using UnityEngine;

namespace Game.Blocks.Movement
{
    public interface IMovementStrategy
    {
        Vector3 GetMovementOffset();
        Vector3 ApplyMovement(Vector3 current, Vector3 target);
    }
}