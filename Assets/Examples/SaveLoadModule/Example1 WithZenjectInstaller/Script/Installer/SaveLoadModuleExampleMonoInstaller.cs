using DataType;
using DataType.Implement;
using UnityEngine;
using Zenject;

namespace SaveLoadModule.Example1
{
    public class SaveLoadModuleExampleMonoInstaller : MonoInstaller
    {
        [SerializeField]
        private ComponentCollectionData _componentCollectionData;
        
        public override void InstallBindings()
        {
            Container.Bind<IReflectionHelperInstaller>().To<ReflectionHelperInstaller>().AsSingle();
            Container.Bind<IReflectionHelper>().To<ReflectionHelper>().AsSingle();

            Container.Bind<ComponentCollectionData>().FromInstance(_componentCollectionData);
            Container.BindInterfacesAndSelfTo<SaveLoadModuleExampleController>().AsSingle();
        }
    }
}