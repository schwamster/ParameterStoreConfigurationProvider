using System.Collections.Generic;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace ParameterStoreConfigurationProvider
{
    public class ParameterStoreConfigurationSource : IConfigurationSource
    {
        public AWSCredentials AwsCredential { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public string Region { get; set; }

        public bool WithDecryption { get; set; }

        public List<ParameterMapping> ParameterMapping;

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ParameterStoreConfigurationProvider(this);
        }
    }
}
