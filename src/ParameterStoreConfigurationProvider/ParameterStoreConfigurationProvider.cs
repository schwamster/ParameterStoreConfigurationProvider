using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System;
using System.Collections.Generic;

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
            IEnumerable<GetParametersResponse> responses;

            if (this.configurationSource.UseDefaultCredentials)
            {
                using (var client = new AmazonSimpleSystemsManagementClient(Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {
                    responses = MappingClientResponseToData(client);
                }
            }
            else
            {
                using (var client = new AmazonSimpleSystemsManagementClient(this.configurationSource.AwsCredential, Amazon.RegionEndpoint.GetBySystemName(this.configurationSource.Region)))
                {

                    responses = MappingClientResponseToData(client);
                }
            }

            MapResults(responses);
        }

        private IEnumerable<GetParametersResponse> MappingClientResponseToData(AmazonSimpleSystemsManagementClient client)
        {
            IEnumerable<GetParametersRequest> requests = PrepareRequests();
            IList<GetParametersResponse> responses = new List<GetParametersResponse>();

            foreach (var request in requests)
            {
                var response = client.GetParametersAsync(request).Result;
                CheckParametersValidity(response);
                responses.Add(response);
            }

            return responses;
        }

        private void CheckParametersValidity(GetParametersResponse response)
        {
            var requiredInvalidParameters = response.InvalidParameters.Where(item =>
                this.configurationSource.ParameterMapping.First(pm =>
                    pm.AwsName == item).Optional == false).ToList();
            if (requiredInvalidParameters.Count > 0)
            {
                var wrongParams = "";
                foreach (var item in requiredInvalidParameters)
                {
                    wrongParams += item + " ";
                }

                throw new Exception("You have requested invalid parameters: " + wrongParams);
            }
        }

        internal IEnumerable<GetParametersRequest> PrepareRequests()
        {
            var names = this.configurationSource.ParameterMapping.Select(x => x.AwsName).ToList();
            const int groupSize = 10;

            var requests = names
                .Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / groupSize, x => x.Item)
                .Select(g => new GetParametersRequest
                {
                    Names = g.ToList(),
                    WithDecryption = this.configurationSource.WithDecryption
                });

            return requests;
        }

        internal void MapResults(IEnumerable<GetParametersResponse> responses)
        {
            this.Data = new Dictionary<string, string>();

            foreach (var response in responses)
            {
                foreach (var parameter in response.Parameters)
                {
                    var parameterMapping =
                        this.configurationSource.ParameterMapping.First(pm => pm.AwsName == parameter.Name);
                    this.Data[parameterMapping.SettingName] = parameter.Value;
                }

                foreach (var parameter in response.InvalidParameters)
                {
                    var parameterMapping =
                        this.configurationSource.ParameterMapping.First(pm => pm.AwsName == parameter);
                    this.Data[parameterMapping.SettingName] = parameterMapping.Default;
                }
            }
        }
    }
}
