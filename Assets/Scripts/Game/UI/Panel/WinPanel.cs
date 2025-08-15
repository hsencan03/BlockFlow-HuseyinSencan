using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Particles;
using UnityEngine;
using Zenject;

namespace Game.UI.Panel
{
    public class WinPanel : GameFinishPanelBase
    {
        [SerializeField] private RectTransform root;
        
        [Inject] private ParticleService particleService;

        public override void Initialize(Canvas canvas)
        {
            base.Initialize(canvas);
            particleService.RegisterParticleType(ParticleId.Win);
        }

        public async override UniTask Show()
        {
            particleService.Play(ParticleId.Win, Vector3.zero, Quaternion.identity);
            await base.Show();
            root.anchoredPosition = new Vector2(0, Screen.height); 
            Tweener tween = root.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
            await tween.AsyncWaitForCompletion();
        }
    }
}