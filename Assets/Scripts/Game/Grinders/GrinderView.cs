using System;
using System.Threading.Tasks;
using Core.Signals;
using Cysharp.Threading.Tasks;
using Game.Blocks;
using Game.Interfaces;
using Game.Particles;
using UnityEngine;
using Zenject;

namespace Game.Grinders
{
    public class GrinderView : MonoBehaviour
    {
        public GrinderModel Data => data;
        
        [SerializeField] private Transform grinderVisuals;
        
        private SignalBus signalBus;
        private IColorProvider<BlockColor> colorProvider;
        private ParticleService particleService;
        private bool shouldRotate;
        
        private GrinderModel data;

        [Inject]
        private void Construct(SignalBus signalBus, IColorProvider<BlockColor> colorProvider, ParticleService particleService)
        {
            this.signalBus = signalBus;
            this.colorProvider = colorProvider;
            this.particleService = particleService;
        }

        private void OnEnable() => signalBus.Subscribe<BlockDestroyedSignal>(OnBlockDestroyed);

        private void OnDisable() => signalBus.TryUnsubscribe<BlockDestroyedSignal>(OnBlockDestroyed);

        public void OnInitialize(GrinderModel grinderModel, bool shouldRotate)
        {
            this.shouldRotate = shouldRotate;
            data = grinderModel;
            if (shouldRotate)
            {
                grinderVisuals.Rotate(Vector3.up, 180f);
            }
            
            grinderVisuals.GetComponent<Renderer>().material.color = colorProvider.GetColor(data.Color);
            particleService.RegisterParticleType(ParticleId.BlockDestroy);
        }
        
        private async void OnBlockDestroyed(BlockDestroyedSignal args)
        {
            if (args.Grinder == data)
            {
                float direction = shouldRotate ? 90 : -90;
                Vector3 localPos = new Vector3(0, 0.5f, 0.5f);
                PooledParticle particle = particleService.Play(ParticleId.BlockDestroy, localPos, Quaternion.Euler(0, direction, 0), false, transform);
                particle.transform.SetParent(null);
                Color targetColor = colorProvider.GetColor(data.Color);
                var particles = particle.GetParticles();
                foreach (var system in particles)
                {
                    SetParticleColor(system, targetColor);
                }
                await UniTask.Delay(TimeSpan.FromSeconds(args.AnimDurationSec));
                await ReleaseParticle(particles, particle);
            }
        }
        
        private void SetParticleColor(ParticleSystem system, Color targetColor)
        {
            var particleRenderer = system.GetComponent<ParticleSystemRenderer>();
            if (particleRenderer != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                particleRenderer.GetPropertyBlock(block);
                block.SetColor("_BaseColor", targetColor); 
                particleRenderer.SetPropertyBlock(block);
            }
        }
        
        private async UniTask ReleaseParticle(ParticleSystem[] particles, PooledParticle particle)
        {
            foreach (var system in particles)
            {
                system.Stop(true);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1));
            particle.Release();
        }
    }
}