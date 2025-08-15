using Core.Data;

namespace Game.Level
{
    public interface ILevelSelector
    {
        int GetNextLevel(GameSaveData saveData);
    }
}