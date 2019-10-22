using System;
using System.Threading.Tasks;

namespace Dioxide.Samples.Proxy
{
    public class DoSomthing : IDoSomthing
    {
        public void DoSomething()
        {
            throw new Exception("very bad exception");
        }

        public int GetInteger(int value)
        {
            return 42 + value;
        }

        public async Task<int> GoTask(string name, int deley)
        {
            await Task.Delay(deley);
            Console.WriteLine($"Hi, {name}");

            return 42;
        }
    }
}
