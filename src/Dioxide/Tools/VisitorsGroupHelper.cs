using Dioxide.Contracts;

namespace Dioxide.Tools
{
    internal class VisitorsGroupHelper
    {
        private string _identifier;
        public string EnterMethod => $"{_identifier}.{nameof(VisitorsGroup.Enter)}";
        public string ExitMethod => $"{_identifier}.{nameof(VisitorsGroup.Exit)}";
        public string CatchMethod => $"{_identifier}.{nameof(VisitorsGroup.Catch)}";
        public string FinalyMethod => $"{_identifier}.{nameof(VisitorsGroup.Finaly)}";

        public VisitorsGroupHelper(string identifier)
        {
            _identifier = identifier;
        }
    }
}
