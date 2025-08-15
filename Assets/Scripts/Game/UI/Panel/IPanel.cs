using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.UI.Panel
{
    public interface IPanel
    {
        void Initialize(Canvas canvas);
        UniTask Show();
        UniTask Hide();
    }
}