using UnityEngine;

namespace Game.Blocks.Movement
{
    public class FrozenMovementStrategy : IMovementStrategy
    {
        public Vector3 GetMovementOffset() => Vector3.zero;

        public Vector3 ApplyMovement(Vector3 current, Vector3 target) => current;
    }
}