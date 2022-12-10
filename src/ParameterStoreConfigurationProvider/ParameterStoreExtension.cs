using System;
using Microsoft.Extensions.Configuration;

namespace ParameterStoreConfigurationProvider
{
    public static class ParameterStoreExtension
    {
        public static IConfigurationBuilder AddParameterStoreConfig(
            this IConfigurationBuilder builder, Action<ParameterStoreConfigurationSource> configureSource)
        {
            return builder.Add(configureSource);
        }
    }
}
