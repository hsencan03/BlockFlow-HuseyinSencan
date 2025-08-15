using Game.Particles;
using Game.ScriptableObjects;
using Game.UI.Panel;
using UnityEngine;
using Zenject;

namespace Core.DI.Installers
{
    [CreateAssetMenu(fileName = "ScriptableObjectInstaller", menuName = "Installers/GameScriptableObjectInstaller")]
    public class GameScriptableObjectInstaller : ScriptableObjectInstaller<GameScriptableObjectInstaller>
    {
        [SerializeField] private PanelHolderSo panelHolder;
        [SerializeField] private BlockConfig blockConfig;
        [SerializeField] private ParticleContainer particleContainer;
        
        public override void InstallBindings()
        {
            Container.BindInstance(panelHolder).AsSingle();
            Container.BindInstance(blockConfig).AsSingle();
            Container.BindInstance(particleContainer).AsSingle();
        }
    }
}