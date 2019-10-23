using System;
using System.Threading.Tasks;

namespace Dioxide.Samples.Proxy
{
    public interface IDoSomthing
    {
        void DoSomething();
        Task<int> GoTask(string name, int delay);
        int GetInteger(int value, int args, string _client);
    }

    public class DoSomthing : IDoSomthing
    {
        public void DoSomething()
        {
            throw new Exception("very bad exception");
        }

        public int GetInteger(int value, int args, string _client)
        {
            throw new Exception("very bad exception");
        }

        public async Task<int> GoTask(string name, int delay)
        {
            await Task.Delay(delay);
            Console.WriteLine($"Hi, {name}");

            return 42;
        }
    }
}
