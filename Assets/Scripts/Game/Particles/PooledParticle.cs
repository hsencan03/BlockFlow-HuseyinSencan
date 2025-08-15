using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticle : MonoBehaviour
    {
        private ParticleSystem[] particles;
        private IObjectPool<PooledParticle> pool;
        private Transform poolParent;

        public void Initialize(IObjectPool<PooledParticle> poolRef, Transform parent)
        {
            pool = poolRef;
            poolParent = parent;
        }

        private void Awake()
        {
            particles = GetComponentsInChildren<ParticleSystem>();
        }
        
        public ParticleSystem[] GetParticles() => particles;
        
        public void Release()
        {
            pool.Release(this);
        }
        
        public void Play(Vector3 position, Quaternion rotation, bool autoRelease = true, Transform parent = null)
        {
            if (parent != null)
            {
                transform.SetParent(parent);
            }
            transform.SetLocalPositionAndRotation(position, rotation);
            foreach (ParticleSystem particle in particles)
            {
                particle.Play();
                if (autoRelease)
                {
                    ReturnWhenDone().Forget();
                }
            }
        }
        
        private async UniTaskVoid ReturnWhenDone()
        {
            try
            {
                await UniTask.WaitWhile(() => particles.Any(I => I.isPlaying), cancellationToken:this.GetCancellationTokenOnDestroy());
                Release();
            }
            catch (Exception)
            {
                //Object destroyed ignored
            }
        }
        
        public void OnGet() => gameObject.SetActive(true);
        public void OnRelease()
        {
            transform.SetParent(poolParent);
            gameObject.SetActive(false);
        }
    }
}