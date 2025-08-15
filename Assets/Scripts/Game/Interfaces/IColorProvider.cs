
using UnityEngine;

namespace Game.Interfaces
{
    public interface IColorProvider<in T> where T : struct, System.Enum
    {
        public Color GetColor(T colorType);
    }
}