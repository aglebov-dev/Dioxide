namespace Dioxide.Dynamic.X2f4d388c32d548ad897bfaa595278bd7
{
    public sealed class IDoSomthing_X0e54b59c7d134ff490db6e663e941cb3 : global::Dioxide.Samples.Proxy.IDoSomthing
    {
        private readonly global::Dioxide.Contracts.InterceptorsGroup _interceptors;
        private readonly global::Dioxide.Samples.Proxy.IDoSomthing _iDoSomthing;
        public IDoSomthing_X0e54b59c7d134ff490db6e663e941cb3(global::Dioxide.Samples.Proxy.IDoSomthing iDoSomthing, global::Dioxide.Samples.Proxy.InformationVisitor informationVisitor, global::Dioxide.Samples.Proxy.MetricsVisitor metricsVisitor)
        {
            _iDoSomthing = iDoSomthing;
            _interceptors = new global::Dioxide.Contracts.InterceptorsGroup();
            _interceptors.Add(informationVisitor as global::Dioxide.Contracts.IProxyInterceptor);
            _interceptors.Add(metricsVisitor as global::Dioxide.Contracts.IProxyInterceptor);
        }

        public void DoSomething()
        {
            var method = "DoSomething";
            var args = new global::Dioxide.Contracts.MethodArgs();
            var result = new global::Dioxide.Contracts.MethodResult();
            try
            {
                _interceptors.Enter(method, args);
                _iDoSomthing.DoSomething();
                _interceptors.Exit(method, args, result);
            }
            catch (global::System.Exception exception)
            {
                var ex = _interceptors.Catch(method, args, exception);
                if (ex == null)
                {
                    throw;
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                _interceptors.Finaly(method, args, result);
            }
        }

        public async global::System.Threading.Tasks.Task<int> GoTask(string name, int deley)
        {
            var method = "GoTask";
            var args = new global::Dioxide.Contracts.MethodArgs();
            var result = new global::Dioxide.Contracts.MethodResult();
            args.Set("name", name);
            args.Set("deley", deley);
            try
            {
                _interceptors.Enter(method, args);
                var methodResult = await _iDoSomthing.GoTask(name, deley);
                result.Set(methodResult);
                _interceptors.Exit(method, args, result);
                return methodResult;
            }
            catch (global::System.Exception exception)
            {
                var overrideResult = _interceptors.CatchOverrideResult(method, args, exception);
                if (overrideResult.HasResult<global::System.Threading.Tasks.Task<int>>())
                {
                    return await overrideResult.GetResultOrDefault<global::System.Threading.Tasks.Task<int>>();
                }

                var ex = _interceptors.Catch(method, args, exception);
                if (ex == null)
                {
                    throw;
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                _interceptors.Finaly(method, args, result);
            }
        }

        public int GetInteger(int value)
        {
            var method = "GetInteger";
            var args = new global::Dioxide.Contracts.MethodArgs();
            var result = new global::Dioxide.Contracts.MethodResult();
            args.Set("value", value);
            try
            {
                _interceptors.Enter(method, args);
                var methodResult = _iDoSomthing.GetInteger(value);
                result.Set(methodResult);
                _interceptors.Exit(method, args, result);
                return methodResult;
            }
            catch (global::System.Exception exception)
            {
                var overrideResult = _interceptors.CatchOverrideResult(method, args, exception);
                if (overrideResult.HasResult<int>())
                {
                    return overrideResult.GetResultOrDefault<int>();
                }

                var ex = _interceptors.Catch(method, args, exception);
                if (ex == null)
                {
                    throw;
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                _interceptors.Finaly(method, args, result);
            }
        }
    }
}