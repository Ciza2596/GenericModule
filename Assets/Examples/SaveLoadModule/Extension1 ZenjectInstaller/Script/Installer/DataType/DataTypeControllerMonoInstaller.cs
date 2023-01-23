using SaveLoadModule;
using Zenject;

public class DataTypeControllerMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IDataTypeController>().To<DataTypeControllerAdapter>().AsSingle();
    }
}