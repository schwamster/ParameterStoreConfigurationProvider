using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ParameterStoreConfigurationProvider;

namespace example_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            
            return WebHost.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostContext, config)=>
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .AddParameterStoreConfig(parameterStoreConfig =>
                        {
                            parameterStoreConfig.ParameterMapping = new List<ParameterMapping>()
                            {
                                new ParameterMapping(){ AwsName = "/somenamespace/somekey", SettingName = "somekey"},
                                new ParameterMapping(){ AwsName = "/somenamespace/someotherkey", SettingName = "someotherkey"},
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
                    })
                    .UseStartup<Startup>()
                    .Build();
        }
    }
}
