using System;
using UnityEngine;

namespace Game.UI.Panel
{
    [CreateAssetMenu(menuName = "Game/Data/UI/PanelHolder")]
    public class PanelHolderSo : ScriptableObject
    {
        [SerializeField] private PanelBehaviour[] panels;
        
        public GameObject GetPanelPrefab<T>() where T : IPanel
        {
            foreach (var panel in panels)
            {
                if (panel is T typedPanel)
                {
                    return panel.gameObject;
                }
            }
            
            Debug.LogError($"Panel of type {typeof(T).Name} not found in PanelHolder.");
            return null;
        }
        
        public GameObject GetPanelPrefab(Type type)
        {
            foreach (var panel in panels)
            {
                if (panel.GetType() == type)
                {
                    return panel.gameObject;
                }
            }
            
            Debug.LogError($"Panel of type {type.Name} not found in PanelHolder.");
            return null;
        }
    }
}