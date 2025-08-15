using Cysharp.Threading.Tasks;

namespace Game.Level.LevelService
{
    public interface ILevelService
    {
        int CurrentLevel { get; }
        UniTask LoadLevelAsync();
        void SetCurrentLevel(int levelIndex);
    }
}