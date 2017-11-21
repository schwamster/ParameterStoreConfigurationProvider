using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ParameterStoreConfigurationProvider
{
    public class ParameterStoreConfigurationSource : IConfigurationSource
    {
        public AWSCredentials AwsCredential { get; set; }

        public string Region { get; set; }

        public bool WithDecryption { get; set; }

        public List<ParameterMapping> ParameterMapping;

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ParameterStoreConfigurationProvider(this);
        }
    }
}
