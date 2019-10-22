namespace Dioxide.Contracts
{
    public interface IMethodResult
    {
        bool HasResult<TResult>();
        TResult GetResultOrDefault<TResult>();
    }
}
