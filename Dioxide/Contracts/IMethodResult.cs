namespace Dioxide.Contracts
{
    public interface IMethodResult
    {
        TResult GetResultOrDefault<TResult>();
    }
}
