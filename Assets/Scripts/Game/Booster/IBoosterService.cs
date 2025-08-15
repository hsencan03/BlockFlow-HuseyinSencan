using Cysharp.Threading.Tasks;
using Game.Booster.Data;

namespace Game.Booster
{
    public interface IBoosterService
    {
        void RegisterBooster(BoosterSo boosterSo);
        UniTask ActivateBooster(BoosterSo boosterSo);
        bool CanActivate(BoosterSo boosterSo);
        BoosterSo[] GetAllBoosters();
    }
}