using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using System.Linq;
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

        private void MappingClientResponseToData(AmazonSimpleSystemsManagementClient client)
        {
            var request = new GetParametersRequest()
            {
                Names = this.configurationSource.ParameterMapping.Select(x => x.AwsName).ToList(),
                WithDecryption = this.configurationSource.WithDecryption
            };
            try
            {
                var response = client.GetParametersAsync(request).Result;
                CheckParametersValidity(response);
                this.Data = response.Parameters.ToDictionary(
                    x => this.configurationSource.ParameterMapping.First(pm => pm.AwsName == x.Name).SettingName,
                    x => x.Value);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override void Load()
        {
            if (this.configurationSource.UseDefaultCredentials)
            {
                using (var client = new AmazonSimpleSystemsManagementClient(Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {
                    MappingClientResponseToData(client);
                }
            }
            else
            {
                using (var client = new AmazonSimpleSystemsManagementClient(this.configurationSource.AwsCredential, Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {
                    MappingClientResponseToData(client);
                }
            }
        }
    }
}
