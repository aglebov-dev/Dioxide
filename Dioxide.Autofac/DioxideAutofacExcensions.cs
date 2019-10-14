using Autofac;
using Dioxide.Contracts;

namespace Dioxide.Autofac
{
    public static class DioxideAutofacExcensions
    {
        public static ContainerBuilder RegistryDioxideTypes(this ContainerBuilder builder, IDioxideResult dioxideResult)
        {
            if (dioxideResult.IsSuccess)
            {
                foreach (var item in dioxideResult.Types)
                {
                    builder.RegisterDecorator(item.GeneratedType, item.OriginType);
                }
            }

            return builder;
        }
    }
}
