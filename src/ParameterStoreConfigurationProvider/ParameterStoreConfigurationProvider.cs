using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;

namespace ParameterStoreConfigurationProvider
{
    public class ParameterStoreConfigurationProvider : ConfigurationProvider
    {
        private readonly ParameterStoreConfigurationSource _configurationSource;

        public ParameterStoreConfigurationProvider(ParameterStoreConfigurationSource configurationSource)
        {
            _configurationSource = configurationSource;
        }

        public override void Load()
        {
            IEnumerable<GetParametersResponse> responses;

            if (_configurationSource.UseDefaultCredentials)
            {
                using (var client = new AmazonSimpleSystemsManagementClient(Amazon.RegionEndpoint.GetBySystemName(_configurationSource.Region)))
                {
                    responses = MappingClientResponseToData(client);
                }
            }
            else
            {
                using (var client = new AmazonSimpleSystemsManagementClient(_configurationSource.AwsCredential, Amazon.RegionEndpoint.GetBySystemName(_configurationSource.Region)))
                {
                    responses = MappingClientResponseToData(client);
                }
            }

            MapResults(responses);
        }

        private IEnumerable<GetParametersResponse> MappingClientResponseToData(IAmazonSimpleSystemsManagement client)
        {
            var requests = PrepareRequests();
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
                _configurationSource.ParameterMapping.First(pm =>
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

        private IEnumerable<GetParametersRequest> PrepareRequests()
        {
            var names = _configurationSource.ParameterMapping.Select(x => x.AwsName).ToList();
            const int groupSize = 10;

            var requests = names
                .Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / groupSize, x => x.Item)
                .Select(g => new GetParametersRequest
                {
                    Names = g.ToList(),
                    WithDecryption = _configurationSource.WithDecryption
                });

            return requests;
        }

        private void MapResults(IEnumerable<GetParametersResponse> responses)
        {
            Data = new Dictionary<string, string>();

            foreach (var response in responses)
            {
                foreach (var parameter in response.Parameters)
                {
                    var parameterMapping =
                        _configurationSource.ParameterMapping.First(pm => pm.AwsName == parameter.Name);
                    Data[parameterMapping.SettingName] = parameter.Value;
                }

                foreach (var parameter in response.InvalidParameters)
                {
                    var parameterMapping =
                        _configurationSource.ParameterMapping.First(pm => pm.AwsName == parameter);
                    Data[parameterMapping.SettingName] = parameterMapping.Default;
                }
            }
        }
    }
}
