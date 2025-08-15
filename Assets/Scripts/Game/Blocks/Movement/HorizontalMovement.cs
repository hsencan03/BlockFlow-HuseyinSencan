using UnityEngine;

namespace Game.Blocks.Movement
{
    public class HorizontalMovement : IMovementStrategy
    {
        public Vector3 GetMovementOffset() => Vector3.up * 0.1f;
        public Vector3 ApplyMovement(Vector3 current, Vector3 target) => new(target.x, current.y, current.z);
    }
}