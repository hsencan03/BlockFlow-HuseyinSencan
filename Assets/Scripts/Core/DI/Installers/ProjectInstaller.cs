using Core.Data;
using Core.Services.AudioService;
using Core.Services.SaveService;
using Core.Services.TimerService;
using Core.Signals;
using Zenject;

namespace Core.DI.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            Container.BindInterfacesAndSelfTo<TimerService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AudioService>().AsSingle();
            Container.Bind<ISaveService<GameSaveData>>().To<PlayerPrefsSaveService<GameSaveData>>().AsSingle();
            
            //Signals
            Container.DeclareSignalWithInterfaces<LevelStateChangedSignal>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<BlockDestroyedSignal>().OptionalSubscriber();
        }
    }
}