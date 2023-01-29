using DataType;
using DataType.Implement;
using Zenject;

public class ReflectionHelperMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IReflectionHelperInstaller>().To<ReflectionHelperInstaller>().AsSingle();
        Container.Bind<IReflectionHelper>().To<ReflectionHelper>().AsSingle();
    }
}