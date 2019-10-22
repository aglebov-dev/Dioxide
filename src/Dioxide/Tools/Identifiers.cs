using Dioxide.Contracts;

namespace Dioxide.Tools
{
    internal class Identifiers
    {
        public string ExceptionVar { get; } = "exception";
        public string MethodVar { get; } = "method";
        public string ArgsVar { get; } = "args";
        public string ResultVar { get; } = "result";
        public string MethodResultVar { get; } = "methodResult";
        public string CarchResultVar { get; } = "ex";

        public string MethodResultSet { get; } = nameof(MethodResult.Set);
        public string MethodArgsSet { get; } = nameof(MethodArgs.Set);
    }
}
