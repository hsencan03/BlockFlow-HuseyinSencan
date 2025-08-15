using UnityEngine;
using Zenject;

namespace Game.UI.Panel
{
    public class PanelFactory
    {
        private readonly DiContainer container;
        private readonly Canvas canvas;

        public PanelFactory(DiContainer container, [Inject(Id = Constants.GameCanvasId)] Canvas canvas)
        {
            this.container = container;
            this.canvas = canvas;
        }

        public IPanel Create(GameObject prefab)
        {
            return container.InstantiatePrefabForComponent<PanelBehaviour>(prefab, canvas.transform);
        }
    }
}