using AudioModule.Implement;
using UnityEngine;
using Zenject;

namespace AudioModule.Example1
{
    public class AudioModuleExampleMonoInstaller : MonoInstaller
    {
        [SerializeField] private AudioResourceDataOverview _audioResourceDataOverview;
        [SerializeField] private ComponentCollectionData _componentCollectionData;
        
        
        public override void InstallBindings()
        {
            Container.Bind<AudioResourceDataOverview>().FromInstance(_audioResourceDataOverview);
            Container.Bind<ComponentCollectionData>().FromInstance(_componentCollectionData);
            Container.BindInterfacesAndSelfTo<AudioModuleExampleController>().AsSingle();
        }
    }
}