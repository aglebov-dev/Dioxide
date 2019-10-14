using System;
using System.Collections.Generic;
using System.Linq;

namespace Dioxide.Contracts
{
    public class MethodArgs: IMethodArgs
    {
        private Dictionary<string, IArg> _parameters { get; set; }

        public MethodArgs()
        {
            _parameters = new Dictionary<string, IArg>();
        }

        public TParam GetParam<TParam>()
        {
            var param = _parameters.Values.OfType<ArgOfT<TParam>>().FirstOrDefault();
            if (param is null)
            {
                throw new ArgumentOutOfRangeException($"Parameter type is {typeof(TParam)} isn't set");
            }

            return param.Value;
        }

        public TParam GetParamByName<TParam>(string name)
        {
            var param = _parameters.Where(x => x.Key == name).Select(x => x.Value).Cast<ArgOfT<TParam>>().FirstOrDefault();
            if (param is null)
            {
                throw new ArgumentOutOfRangeException($"Parameter type is {typeof(TParam)} and name {name} isn't set");
            }

            return param.Value;
        }

        public void Set<T>(string name, T value)
        {
            _parameters[name] = new ArgOfT<T>() { Value = value };
        }

        private interface IArg { }

        private class ArgOfT<T> : IArg
        {
            public T Value { get; set; }
        }
    }
}
