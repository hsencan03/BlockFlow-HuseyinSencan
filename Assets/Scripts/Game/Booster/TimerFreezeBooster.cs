using System;
using Core.Services.TimerService;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Booster.Data;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Booster
{
    public class TimerFreezeBooster : IBooster
    {
        public async UniTask Apply(BoosterSo boosterData)
        {
            TimerFreezeBoosterSo data = (TimerFreezeBoosterSo)boosterData;

            Timer timer = data.TimerService.GetTimer(Constants.LevelTimerID);
            timer.Pause();
            Image effect = await CreateUIEffect(data);
            await UniTask.Delay(TimeSpan.FromSeconds(data.durationInSeconds));
            await DestroyUIEffect(effect);
            timer.Resume();
            
            await UniTask.Yield();
        }

        private async UniTask<Image> CreateUIEffect(TimerFreezeBoosterSo data)
        {
            Image uiEffect = Object.Instantiate(data.uiEffect, data.canvas.transform).GetComponent<Image>();
            uiEffect.gameObject.SetActive(true);
            await uiEffect.DOFade(1, 0.75f).From(0).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            return uiEffect;
        }

        private async UniTask DestroyUIEffect(Image uiEffect)
        {
            await uiEffect.DOFade(0, 0.25f).From(1).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            Object.Destroy(uiEffect);
        }
    }
}