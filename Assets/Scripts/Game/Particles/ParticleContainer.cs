using System.Linq;
using UnityEngine;

namespace Game.Particles
{
    [System.Serializable]
    public struct ParticleEntry
    {
        public ParticleId id;
        public PooledParticle prefab;
    }
    
    [CreateAssetMenu(menuName = "Game/Data/Particle/Container")]
    public class ParticleContainer : ScriptableObject
    {
        [SerializeField] private ParticleEntry[] particles;
        
        public PooledParticle GetPrefab(ParticleId id)
        {
            return particles.FirstOrDefault(I => I.id == id).prefab;
        }
    }
}