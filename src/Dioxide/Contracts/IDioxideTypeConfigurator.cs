namespace Dioxide.Contracts
{
    public interface IDioxideTypeConfigurator { }

    public interface IDioxideTypeConfigurator<T> : IDioxideTypeConfigurator
        where T: class
    {
        IDioxideTypeConfigurator<T> With<V>() where V : class, T;
    }
}
