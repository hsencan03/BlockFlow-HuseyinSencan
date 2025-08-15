using System.Numerics;
using Game.Data;
using Game.ScriptableObjects;
using UnityEngine;
using Zenject;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Game.Grinders
{
    public class GrinderFactory
    {
        private DiContainer container;
        private GridPrefabsConfig gridPrefabsConfig;
        
        public GrinderFactory(DiContainer container, GridPrefabsConfig gridPrefabsConfig)
        {
            this.container = container;
            this.gridPrefabsConfig = gridPrefabsConfig;
        }
        
        public GrinderView Create(GrinderModel model, Transform parent, Vector3 position, Quaternion rotation, bool shouldRotate = false)
        {
            GrinderView grinderView = container.InstantiatePrefabForComponent<GrinderView>(gridPrefabsConfig.grinderPrefab);
            grinderView.transform.SetParent(parent);
            grinderView.transform.localPosition = position;
            grinderView.transform.localRotation = rotation;
            grinderView.OnInitialize(model, shouldRotate);
            return grinderView;
        }
    }
}