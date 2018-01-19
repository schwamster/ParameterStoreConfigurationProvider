using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System;

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

            GetParametersResponse response;

            if (this.configurationSource.UseDefaultCredentials)
            {
                using (var client = new AmazonSimpleSystemsManagementClient(Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {
                  response = MappingClientResponseToData(client);
                }
            }
            else
            {
                using (var client = new AmazonSimpleSystemsManagementClient(this.configurationSource.AwsCredential, Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {

                    response = MappingClientResponseToData(client);
                }
            }

            MapResult(response);
        }

        private GetParametersResponse MappingClientResponseToData(AmazonSimpleSystemsManagementClient client)
        {
            GetParametersRequest request = PrepareRequest();
            GetParametersResponse response;
            try
            {
                response = client.GetParametersAsync(request).Result;
                CheckParametersValidity(response);
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        private void CheckParametersValidity(GetParametersResponse response)
        {
            if (response.InvalidParameters.Count > 0)
            {
                String wrongParams = "";
                foreach (var item in response.InvalidParameters)
                {
                    wrongParams = item + " ";
                }

                throw new Exception("You have requested invalid parameters " + wrongParams);
            }
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
