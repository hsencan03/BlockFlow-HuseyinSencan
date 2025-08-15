using System;
using Cysharp.Threading.Tasks;

namespace Game.UI.Panel
{
    public interface IPanelService
    {
        public UniTask<T> ShowPanel<T>() where T : IPanel;

        public UniTask<IPanel> ShowPanel(Type type);

        public UniTask HidePanel<T>() where T : IPanel;

        public UniTask HideAllPanels();
    }
}