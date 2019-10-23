namespace Dioxide.Dynamic.X36cedcf0195d4cc890ca7d9fc8a04fa6
{
    public sealed class IDoSomthing_X290665897f994f1a9874d7ce855348d4 : global::Dioxide.Samples.Proxy.IDoSomthing
    {
        private readonly global::Dioxide.Contracts.InterceptorsGroup _interseptors;
        private readonly global::Dioxide.Samples.Proxy.IDoSomthing _client;
        public IDoSomthing_X290665897f994f1a9874d7ce855348d4(global::Dioxide.Samples.Proxy.IDoSomthing client, global::Dioxide.Samples.Proxy.Interseptors.LogInterceptor logInterceptor, global::Dioxide.Samples.Proxy.Interseptors.MetricsInterceptor metricsInterceptor, global::Dioxide.Samples.Proxy.Interseptors.ForgiveMistakesInterceptor forgiveMistakesInterceptor)
        {
            _client = client;
            _interseptors = new global::Dioxide.Contracts.InterceptorsGroup();
            _interseptors.Add(logInterceptor as global::Dioxide.Contracts.IProxyInterceptor);
            _interseptors.Add(metricsInterceptor as global::Dioxide.Contracts.IProxyInterceptor);
            _interseptors.Add(forgiveMistakesInterceptor as global::Dioxide.Contracts.IProxyInterceptor);
        }

        public void DoSomething()
        {
            var method = "DoSomething";
            var args = new global::Dioxide.Contracts.MethodArgs();
            var result = new global::Dioxide.Contracts.MethodResult();
            try
            {
                _interseptors.Enter(method, args);
                _client.DoSomething();
                _interseptors.Exit(method, args, result);
            }
            catch (global::System.Exception ex)
            {
                var exception = _interseptors.Catch(method, args, ex);
                if (exception == null)
                {
                    throw;
                }
                else
                {
                    throw exception;
                }
            }
            finally
            {
                _interseptors.Finaly(method, args, result);
            }
        }

        public async global::System.Threading.Tasks.Task<int> GoTask(string name, int delay)
        {
            var method = "GoTask";
            var args = new global::Dioxide.Contracts.MethodArgs();
            var result = new global::Dioxide.Contracts.MethodResult();
            args.Set("name", name);
            args.Set("delay", delay);
            try
            {
                _interseptors.Enter(method, args);
                var methodResult = await _client.GoTask(name, delay);
                result.Set(methodResult);
                _interseptors.Exit(method, args, result);
                return methodResult;
            }
            catch (global::System.Exception ex)
            {
                var overrideResult = _interseptors.CatchOverrideResult(method, args, ex);
                if (overrideResult.HasResult<global::System.Threading.Tasks.Task<int>>())
                {
                    _interseptors.Exit(method, args, overrideResult);
                    return await overrideResult.GetResultOrDefault<global::System.Threading.Tasks.Task<int>>();
                }

                var exception = _interseptors.Catch(method, args, ex);
                if (exception == null)
                {
                    throw;
                }
                else
                {
                    throw exception;
                }
            }
            finally
            {
                _interseptors.Finaly(method, args, result);
            }
        }

        public int GetInteger(int value, int args, string _client1)
        {
            var method = "GetInteger";
            var args1 = new global::Dioxide.Contracts.MethodArgs();
            var result = new global::Dioxide.Contracts.MethodResult();
            args1.Set("value", value);
            args1.Set("args", args);
            args1.Set("_client", _client1);
            try
            {
                _interseptors.Enter(method, args1);
                var methodResult = _client.GetInteger(value, args, _client1);
                result.Set(methodResult);
                _interseptors.Exit(method, args1, result);
                return methodResult;
            }
            catch (global::System.Exception ex)
            {
                var overrideResult = _interseptors.CatchOverrideResult(method, args1, ex);
                if (overrideResult.HasResult<int>())
                {
                    _interseptors.Exit(method, args1, overrideResult);
                    return overrideResult.GetResultOrDefault<int>();
                }

                var exception = _interseptors.Catch(method, args1, ex);
                if (exception == null)
                {
                    throw;
                }
                else
                {
                    throw exception;
                }
            }
            finally
            {
                _interseptors.Finaly(method, args1, result);
            }
        }
    }
}