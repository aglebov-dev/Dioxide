using Dioxide.Contracts;

namespace Dioxide.InternalTypes
{
    internal class DioxideResult : IDioxideResult
    {
        public bool IsSuccess { get; }
        public IDioxideTypeBuilderResult[] Types { get; }

        public DioxideResult(bool isSuccess, IDioxideTypeBuilderResult[] types)
        {
            IsSuccess = isSuccess;
            Types = types;
        }
    }
}
