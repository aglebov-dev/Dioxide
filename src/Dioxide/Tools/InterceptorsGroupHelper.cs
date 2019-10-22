using Dioxide.Contracts;

namespace Dioxide.Tools
{
    internal class InterceptorsGroupHelper
    {
        private string _identifier;
        public string EnterMethod => $"{_identifier}.{nameof(InterceptorsGroup.Enter)}";
        public string ExitMethod => $"{_identifier}.{nameof(InterceptorsGroup.Exit)}";
        public string CatchMethod => $"{_identifier}.{nameof(InterceptorsGroup.Catch)}";
        public string FinalyMethod => $"{_identifier}.{nameof(InterceptorsGroup.Finaly)}";
        public string CatchOverrideResultMethod => $"{_identifier}.{nameof(InterceptorsGroup.CatchOverrideResult)}";

        public InterceptorsGroupHelper(string identifier)
        {
            _identifier = identifier;
        }
    }
}
