namespace Dioxide.Contracts
{
    public class MethodResult: IMethodResult
    {
        private IMethodResultOfT _result;

        public bool HasResult<TResult>()
        {
            return _result is MethodResultOfT<TResult>;
        }

        public TResult GetResultOrDefault<TResult>()
        {
            if (_result is MethodResultOfT<TResult> result)
            {
                return result.Value;
            }

            return default;
        }

        public void Set<T>(T value)
        {
            _result = new MethodResultOfT<T> { Value = value };
        }

        private interface IMethodResultOfT 
        {
            object GetResult();
        }

        private class MethodResultOfT<T> : IMethodResultOfT
        {
            public T Value { get; set; }

            public object GetResult()
            {
                return Value;
            }
        }
    }
}
