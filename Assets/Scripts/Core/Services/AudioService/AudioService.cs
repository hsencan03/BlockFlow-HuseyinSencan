using System;
using Core.Signals;
using Cysharp.Threading.Tasks;
using Game.Audio;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Core.Services.AudioService
{
    public class AudioService : IAudioService, IInitializable, IDisposable
    {
        private SignalBus signalBus;
        private AudioClipContainer container;

        private AudioSource audioSource;

        public AudioService(SignalBus signalBus, AudioClipContainer container)
        {
            this.signalBus = signalBus;
            this.container = container;
        }
        
        public void Initialize()
        {
            container.Init();
            audioSource = new GameObject("Audio Source").AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(audioSource);
            
            signalBus.Subscribe<ISignalSfxPlayer>(OnSFXSignal);
        }
        
        public void PlaySFX(SfxId id)
        {
            AudioClip clip = container.GetClip(id);
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        public void Dispose()
        {
            signalBus.TryUnsubscribe<ISignalSfxPlayer>(OnSFXSignal);
            Object.Destroy(audioSource);
        }

        private async void OnSFXSignal(ISignalSfxPlayer args)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(args.DelaySeconds));
            PlaySFX(args.Id);
        }
    }
}