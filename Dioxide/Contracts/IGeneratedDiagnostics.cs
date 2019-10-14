using System;

namespace Dioxide.Contracts
{
    public interface IGeneratorDiagnostics 
    {
        Action<string> CompilationResult { get; }
    }
}
