namespace Dioxide.Contracts
{
    public interface ITypeConfigurator { }

    public interface ITypeConfigurator<T> : ITypeConfigurator
        where T: class
    {
        ITypeConfigurator<T> With<V>() where V : class, T;
    }
}
