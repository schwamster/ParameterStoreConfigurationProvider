using example_api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using ParameterStoreConfigurationProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Xunit;

namespace example_api_test
{
    public class ConfigurationTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ConfigurationTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddParameterStoreConfig(parameterStoreConfig =>
                {
                    parameterStoreConfig.ParameterMapping = new List<ParameterMapping>()
                    {
                        new ParameterMapping(){ AwsName = "/somenamespace/somekey", SettingName = "somekey"},
                    };
                    parameterStoreConfig.Region = "eu-west-1";
                    parameterStoreConfig.AwsCredential = new Amazon.Runtime.StoredProfileAWSCredentials();
                })
                .AddParameterStoreConfig(parameterStoreConfig =>
                {
                    parameterStoreConfig.ParameterMapping = new List<ParameterMapping>()
                    {
                        new ParameterMapping(){ AwsName = "/somenamespace/somesecurekey", SettingName = "somesecurekey"}
                    };
                    parameterStoreConfig.WithDecryption = true;
                    parameterStoreConfig.Region = "eu-west-1";
                    parameterStoreConfig.AwsCredential = new Amazon.Runtime.StoredProfileAWSCredentials();
                });
            var config = builder.Build();

            _server = new TestServer(new WebHostBuilder()
                .UseConfiguration(config)
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async System.Threading.Tasks.Task PipelineSetupTestAsync_GetRegularSetting()
        {
            // Act
            var response = await _client.GetAsync("/api/Configuration/somekey");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("somevalue", responseString);
        }


        [Fact]
        public async System.Threading.Tasks.Task PipelineSetupTestAsync_GetSecureSetting()
        {
            // Act
            var response = await _client.GetAsync("/api/Configuration/somesecurekey");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("somesecurevalue", responseString);
        }

    }
}
