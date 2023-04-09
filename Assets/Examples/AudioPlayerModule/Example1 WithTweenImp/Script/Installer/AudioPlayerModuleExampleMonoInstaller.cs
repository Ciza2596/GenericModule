using CizaAudioModule.Implement;
using GameCore.Implement;
using GameCore.Infrastructure;
using UnityEngine;
using Zenject;

namespace CizaAudioPlayerModule.Example1
{
    public class AudioPlayerModuleExampleMonoInstaller : MonoInstaller
    {
        // [SerializeField] private AudioResourceDataOverview _audioResourceDataOverview;
        // [SerializeField] private ComponentCollectionData _componentCollectionData;
        //
        //
        // public override void InstallBindings()
        // {
        //     Container.Bind<ITimeModule>().To<TimeModule>().AsSingle();
        //     Container.BindInterfacesAndSelfTo<TimerModule>().AsSingle();
        //     Container.BindInterfacesAndSelfTo<AudioPlayerModuleExampleController>().AsSingle();
        //     Container.Bind<ComponentCollectionData>().FromInstance(_componentCollectionData);
        //     Container.Bind<AudioResourceDataOverview>().FromInstance(_audioResourceDataOverview);
        // }
    }
}