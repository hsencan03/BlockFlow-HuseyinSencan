using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.UI.Panel
{
    public class LosePanel : GameFinishPanelBase
    {
        [SerializeField] private RectTransform root;
        
        public async override UniTask Show()
        {
            await base.Show();
            root.anchoredPosition = new Vector2(0, -Screen.height); 
            Tweener tween = root.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
            await tween.AsyncWaitForCompletion();
        }
    }
}