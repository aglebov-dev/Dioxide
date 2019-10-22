namespace Dioxide.Contracts
{
    public interface IDioxideResult
    {
        bool IsSuccess { get; }
        IDioxideTypeBuilderResult[] Types { get; }
    }
}
