using Game.Audio;
using Game.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace Core.DI.Installers
{
    [CreateAssetMenu(fileName = "ScriptableObjectInstaller", menuName = "Installers/ProjectScriptableObjectInstaller")]
    public class ProjectScriptableObjectInstaller : ScriptableObjectInstaller<ProjectScriptableObjectInstaller>
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private GridPrefabsConfig gridPrefabsConfig;
        [SerializeField] private AudioClipContainer audioClipContainer;
        
    
        public override void InstallBindings()
        {
            Container.BindInstance(config).AsSingle();
            Container.BindInstance(gridPrefabsConfig).AsSingle();
            Container.BindInstance(audioClipContainer).AsSingle();
        }
    }
}