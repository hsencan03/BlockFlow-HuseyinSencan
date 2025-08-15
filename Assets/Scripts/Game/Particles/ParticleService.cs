using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace Game.Particles
{
    public class ParticleService
    {
        private Transform parentContainer;
        private ParticleContainer particleContainer;
        
        private readonly Dictionary<ParticleId, IObjectPool<PooledParticle>> particlePools = new();

        public ParticleService([Inject(Id = Constants.ParticleParentTransformId)] Transform parentContainer, ParticleContainer particleContainer)
        {
            this.parentContainer = parentContainer;
            this.particleContainer = particleContainer;
        }
        
        public bool RegisterParticleType(ParticleId id, int defaultCapacity = 5, int maxSize = 10)
        {
            if (particlePools.ContainsKey(id))
                return false;

            PooledParticle particlePrefab = particleContainer.GetPrefab(id);
            
            if(particlePrefab == null)
                return false;
            
            IObjectPool<PooledParticle> pool = null;
            pool = new ObjectPool<PooledParticle>(
                createFunc: () =>
                {
                    var particle = Object.Instantiate(particlePrefab.gameObject, parentContainer).GetComponent<PooledParticle>();
                    particle.Initialize(pool, parentContainer);
                    particle.OnRelease();
                    return particle;
                },
                actionOnGet: p => p.OnGet(),
                actionOnRelease: p => p.OnRelease(),
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );

            particlePools[id] = pool;
            return true;
        }

        public PooledParticle Play(ParticleId id, Vector3 position, Quaternion rotation, bool autoRelease = true, Transform parent = null)
        {
            if (!particlePools.TryGetValue(id, out var pool))
            {
                Debug.LogWarning($"Particle type '{id}' not registered.");
                return null;
            }

            var particle = pool.Get();
            particle.Play(position, rotation, autoRelease, parent);
            return particle;
        }
    }
}