using System;

namespace Game.UI.Panel
{
    public abstract class GameFinishPanelBase : PanelBehaviour
    {
        public event Action OnButtonPressed;
        
        public void InvokeButtonPressed()
        {
            OnButtonPressed?.Invoke();
        }
    }
}