using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ParameterStoreConfigurationProvider;

namespace example_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildHost(args).Run();
        }

        public static IHost BuildHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostContext, config) =>
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
                            parameterStoreConfig.UseDefaultCredentials = true;
                            //   parameterStoreConfig.AwsCredential = new Amazon.Runtime.StoredProfileAWSCredentials();
                        })
                        .AddParameterStoreConfig(parameterStoreConfig =>
                        {
                            parameterStoreConfig.ParameterMapping = new List<ParameterMapping>()
                            {
                                new ParameterMapping(){ AwsName = "/somenamespace/somesecurekey", SettingName = "somesecurekey"}
                            };
                            parameterStoreConfig.WithDecryption = true;
                            parameterStoreConfig.Region = "eu-west-1";
                            parameterStoreConfig.UseDefaultCredentials = true;
                            //    parameterStoreConfig.AwsCredential = new Amazon.Runtime.StoredProfileAWSCredentials();
                        });
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                    .Build();
        }
    }
}
