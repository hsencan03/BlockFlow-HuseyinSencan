using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Booster.Data;
using Zenject;

namespace Game.Booster
{
    public class BoosterService : IBoosterService
    {
        private readonly List<BoosterSo> registeredBoosters = new();

        private DiContainer diContainer;
        
        public BoosterService(DiContainer container)
        {
            diContainer = container;
        }
        
        public void RegisterBooster(BoosterSo boosterSo)
        {
            if (boosterSo == null || registeredBoosters.Contains(boosterSo))
                return;

            diContainer.Inject(boosterSo);
            registeredBoosters.Add(boosterSo);
        }

        public async UniTask ActivateBooster(BoosterSo boosterSo)
        {
            if (boosterSo == null || !CanActivate(boosterSo))
                return;

            IBooster instance = boosterSo.CreateInstance();
            await instance.Apply(boosterSo);
        }

        public bool CanActivate(BoosterSo boosterSo)
        {
            return registeredBoosters.Contains(boosterSo);
        }

        public BoosterSo[] GetAllBoosters()
        {
            return registeredBoosters.ToArray();
        }
    }
}