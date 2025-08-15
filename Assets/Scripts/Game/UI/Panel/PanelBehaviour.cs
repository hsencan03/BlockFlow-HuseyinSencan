using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.UI.Panel
{
    public abstract class PanelBehaviour : MonoBehaviour, IPanel
    {
        public virtual void Initialize(Canvas canvas)
        {
            transform.SetParent(canvas.transform, false);
        }

        public virtual UniTask Show()
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public virtual UniTask Hide()
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
    }
}