namespace Dioxide.Contracts
{
    public interface IGenerateResult
    {
        bool IsSuccess { get; }
        ITypeBuilderResult[] Types { get; }
    }
}
