using System;

namespace Dioxide.Contracts
{
    public interface ICatchResultsOverrider
    {
        IMethodResult CatchOverrideResult(string methodName, IMethodArgs args, Exception exception);
    }
}
