using Core.Services.LevelLoader;
using Game;
using Game.Blocks;
using Game.Booster;
using Game.Grid;
using Game.Grinders;
using Game.Level;
using Game.Level.LevelService;
using Game.Particles;
using Game.UI.Panel;
using UnityEngine;
using Zenject;

namespace Core.DI.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Transform gridParent;
        [SerializeField] private Transform particleParent;
        
        [SerializeField] private Canvas gameCanvas;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelService>().AsSingle();
            Container.BindInterfacesAndSelfTo<JsonLevelLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<GridSystem>().AsCached();
            Container.BindInterfacesAndSelfTo<PanelService>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoosterService>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<RecentLevelAvoidingSelector>().AsCached();
            
            Container.Bind<ParticleService>().AsCached().Lazy();
            
            Container.Bind<BlockFactory>().AsCached();
            Container.Bind<GrinderFactory>().AsCached();
            Container.Bind<PanelFactory>().AsCached();

            Container.BindInterfacesTo<BlockColorProvider>().AsSingle();
            
            Container.BindInstance(gridParent).WithId(Game.Constants.GridParentTransformId);
            Container.BindInstance(particleParent).WithId(Game.Constants.ParticleParentTransformId);
            Container.BindInstance(gameCanvas).WithId(Game.Constants.GameCanvasId);
        }
    }
}