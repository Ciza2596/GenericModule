using SaveLoadModule;
using SaveLoadModule.Implement;
using Zenject;

public class DataTypeControllerMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IDataTypeController>().To<DataTypeControllerAdapter>().AsSingle();
    }
}