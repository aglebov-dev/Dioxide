using Dioxide.Contracts;

namespace Dioxide.InternalTypes
{
    internal class GenerateResult : IGenerateResult
    {
        public bool IsSuccess { get; }
        public ITypeBuilderResult[] Types { get; }

        public GenerateResult(bool isSuccess, ITypeBuilderResult[] types)
        {
            IsSuccess = isSuccess;
            Types = types;
        }
    }
}
