using UnityEngine;

namespace Game.Booster.Data
{
    public abstract class BoosterSo : ScriptableObject
    {
        public abstract IBooster CreateInstance();
    }
}