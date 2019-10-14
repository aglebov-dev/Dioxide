using System;

namespace Dioxide
{
    public class BuildResult
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public BuildResultType[] Types { get; }

        public BuildResult(bool isSuccess, string error, BuildResultType[] types)
        {
            IsSuccess = isSuccess;
            Error = error;
            Types = types;
        }
    }

    public class BuildResultType
    {
        public Type Type { get; }
        public Type OriginType { get; }

        public BuildResultType(Type type, Type originType)
        {
            Type = type;
            OriginType = originType;
        }
    }
}
