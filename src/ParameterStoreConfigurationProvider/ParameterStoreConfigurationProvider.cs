using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

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
            GetParametersRequest request = PrepareRequest();

            GetParametersResponse response;

            if (this.configurationSource.UseDefaultCredentials)
            {
                using (var client = new AmazonSimpleSystemsManagementClient(Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {


                    response = client.GetParametersAsync(request).Result;
                }
            }
            else
            {
                using (var client = new AmazonSimpleSystemsManagementClient(this.configurationSource.AwsCredential, Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {

                    response = client.GetParametersAsync(request).Result;
                }
            }

            MapResult(response);
        }

        internal GetParametersRequest PrepareRequest()
        {
            return new GetParametersRequest()
            {
                Names = this.configurationSource.ParameterMapping.Select(x => x.AwsName).ToList(),
                WithDecryption = this.configurationSource.WithDecryption
            };
        }

        internal void MapResult(GetParametersResponse response)
        {
            this.Data = response.Parameters.ToDictionary(
                                    x => this.configurationSource.ParameterMapping.First(pm => pm.AwsName == x.Name).SettingName,
                                    x => x.Value);
        }
    }
}
