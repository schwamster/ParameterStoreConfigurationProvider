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
        private TestServer _server;
        private HttpClient _client;

        [Fact]
        public async System.Threading.Tasks.Task PipelineSetupTestAsync_GetRegularSetting()
        {
            // Act
            SetupValidServer();
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
            SetupValidServer();
            var response = await _client.GetAsync("/api/Configuration/somesecurekey");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("somesecurevalue", responseString);
        }

        [Fact]
        public async System.Threading.Tasks.Task PipelineSetupTestAsync_GetDefaultSetting()
        {
            // Act
            SetupValidServer();
            var response = await _client.GetAsync("/api/Configuration/nonexistantkey");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("DefaultValue", responseString);
        }

        [Fact]
        public async System.Threading.Tasks.Task PipelineSetupTestAsync_GetNullDefaultSetting()
        {
            // Act
            SetupValidServer();
            var response = await _client.GetAsync("/api/Configuration/anothernonexistantkey");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("***ValueWasNull***", responseString);
        }

        [Fact]
        public void PipelineSetupTestAsync_MissingNonOptionalSetting()
        {
            // Act
            var ex = Assert.Throws<Exception>(() => SetupInvalidServer());

            //Assert
            Assert.Equal("You have requested invalid parameters: /somenamespace/anothernonexistantkey /somenamespace/yetanothernonexistantkey ", ex.Message);
        }

        private void SetupValidServer()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddParameterStoreConfig(parameterStoreConfig =>
                {
                    parameterStoreConfig.ParameterMapping = new List<ParameterMapping>()
                    {
                        new ParameterMapping
                        {
                            AwsName = "/somenamespace/somekey",
                            SettingName = "somekey"
                        }
                    };
                    parameterStoreConfig.Region = "eu-west-1";
                    parameterStoreConfig.UseDefaultCredentials = true;
                })
                .AddParameterStoreConfig(parameterStoreConfig =>
                {
                    parameterStoreConfig.ParameterMapping = new List<ParameterMapping>()
                    {
                        new ParameterMapping
                        {
                            AwsName = "/somenamespace/somesecurekey",
                            SettingName = "somesecurekey"
                        },
                        new ParameterMapping
                        {
                            AwsName = "/somenamespace/nonexistantkey",
                            SettingName = "nonexistantkey",
                            Optional = true,
                            Default = "DefaultValue"
                        },
                        new ParameterMapping
                        {
                            AwsName = "/somenamespace/anothernonexistantkey",
                            SettingName = "anothernonexistantkey",
                            Optional = true
                        }
                    };
                    parameterStoreConfig.WithDecryption = true;
                    parameterStoreConfig.Region = "eu-west-1";
                    parameterStoreConfig.UseDefaultCredentials = true;
                });
            var config = builder.Build();

            _server = new TestServer(new WebHostBuilder()
                .UseConfiguration(config)
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private void SetupInvalidServer()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddParameterStoreConfig(parameterStoreConfig =>
                {
                    parameterStoreConfig.ParameterMapping = new List<ParameterMapping>()
                    {
                        new ParameterMapping
                        {
                            AwsName = "/somenamespace/somesecurekey",
                            SettingName = "somesecurekey"
                        },
                        new ParameterMapping
                        {
                            AwsName = "/somenamespace/nonexistantkey",
                            SettingName = "nonexistantkey",
                            Optional = true,
                            Default = "DefaultValue"
                        },
                        new ParameterMapping
                        {
                            AwsName = "/somenamespace/anothernonexistantkey",
                            SettingName = "anothernonexistantkey"
                        },
                        new ParameterMapping
                        {
                            AwsName = "/somenamespace/yetanothernonexistantkey",
                            SettingName = "yetanothernonexistantkey"
                        }
                    };
                    parameterStoreConfig.WithDecryption = true;
                    parameterStoreConfig.Region = "eu-west-1";
                    parameterStoreConfig.UseDefaultCredentials = true;
                });
            var config = builder.Build();

            _server = new TestServer(new WebHostBuilder()
                .UseConfiguration(config)
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }
    }
}
