using Cysharp.Threading.Tasks;
using Game.Data;

namespace Core.Services.LevelLoader
{
    public interface ILevelLoader
    {
        UniTask<LevelData> LoadLevelAsync(string levelName);
    }
}