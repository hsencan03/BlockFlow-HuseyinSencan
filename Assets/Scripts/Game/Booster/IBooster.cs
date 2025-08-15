using Cysharp.Threading.Tasks;
using Game.Booster.Data;

namespace Game.Booster
{
    public interface IBooster
    {
        UniTask Apply(BoosterSo boosterData);
    }
}