using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ParameterStoreConfigurationProvider
{

    public class ParameterStoreConfigurationProvider : ConfigurationProvider
    {
        private readonly ParameterStoreConfigurationSource configurationSource;

        public ParameterStoreConfigurationProvider(ParameterStoreConfigurationSource configurationSource)
        {
            this.configurationSource = configurationSource;
        }

        public override void Load()
        {
            if (this.configurationSource.UseDefaultCredentials)
            {
                using (var client = new AmazonSimpleSystemsManagementClient(Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {
                    var request = new GetParametersRequest()
                    {
                        Names = this.configurationSource.ParameterMapping.Select(x => x.AwsName).ToList(),
                        WithDecryption = this.configurationSource.WithDecryption
                    };

                    var response = client.GetParametersAsync(request).Result;

                    this.Data = response.Parameters.ToDictionary(
                        x => this.configurationSource.ParameterMapping.First(pm => pm.AwsName == x.Name).SettingName,
                        x => x.Value);
                }
            }
            else
            {
                using (var client = new AmazonSimpleSystemsManagementClient(this.configurationSource.AwsCredential, Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {
                    var request = new GetParametersRequest()
                    {
                        Names = this.configurationSource.ParameterMapping.Select(x => x.AwsName).ToList(),
                        WithDecryption = this.configurationSource.WithDecryption
                    };

                    var response = client.GetParametersAsync(request).Result;

                    this.Data = response.Parameters.ToDictionary(
                        x => this.configurationSource.ParameterMapping.First(pm => pm.AwsName == x.Name).SettingName,
                        x => x.Value);
                }
            }
        }
    }
}
