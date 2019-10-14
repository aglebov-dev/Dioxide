using System;
using System.Threading.Tasks;

namespace Dioxide.Samples.Decorator
{
    public class DoSomthing : IDoSomthing
    {
        public void DoSomething()
        {
            throw new Exception("very bad exception");
        }

        public async Task<int> GoTask(string name, int deley)
        {
            await Task.Delay(deley);
            Console.WriteLine($"Hi, {name}");

            return 42;
        }
    }
}
