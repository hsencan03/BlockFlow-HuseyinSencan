using Core.Services.TimerService;
using UnityEngine;
using Zenject;

namespace Game.Booster.Data
{
    [CreateAssetMenu(menuName = "Game/Data/Booster/TimerFreeze")]
    public class TimerFreezeBoosterSo : BoosterSo
    {
        public float durationInSeconds;
        public GameObject uiEffect;

        [Inject] public ITimerService TimerService;
        [Inject(Id = Constants.GameCanvasId), HideInInspector] public Canvas canvas;
        
        public override IBooster CreateInstance()
        {
            return new TimerFreezeBooster();
        }
    }
}