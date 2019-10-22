using System.Threading.Tasks;

namespace Dioxide.Samples.Proxy
{
    public interface IDoSomthing
    {
        void DoSomething();
        Task<int> GoTask(string name, int deley);
        int GetInteger(int value);
    }
}
