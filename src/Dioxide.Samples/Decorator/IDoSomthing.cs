using System.Threading.Tasks;

namespace Dioxide.Samples.Decorator
{
    public interface IDoSomthing
    {
        void DoSomething();
        Task<int> GoTask(string name, int deley);
    }
}
