namespace Dioxide.Contracts
{
    public interface IMethodArgs
    {
        TParam GetParam<TParam>();
        TParam GetParamByName<TParam>(string name);
    }
}
