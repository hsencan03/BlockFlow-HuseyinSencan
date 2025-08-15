using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game/Config/Block")]
    public class BlockConfig : ScriptableObject
    {
        public float maxVelocity = 10;
        public float destroyAnimDurationSec = 0.5f;
    }
}