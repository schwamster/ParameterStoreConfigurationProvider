
# ParameterStoreConfigurationProvider

[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg)](https://www.nuget.org/packages/ParameterStoreConfigurationProvider/)

Enrich your configuration with plain text or secure settings from AWS ParameterStore.

## Example

Example configuration for a asp.net core project:

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

# Finding your way in the solution

in /src you find two projects, one example-api and the src for the nuget package itself
in /test you find the unittests

To use the provided samples you have to will have to setup 3 parameters in the ParameterStore

/somenamespace/somekey
/somenamespace/someotherkey
/somenamespace/somesecurekey => This needs to be set up as a secure string, which requires a KMS-Encryption key

You will also have set up and save a local default aws profile on the computer if you want to use StoredProfileAWSCredentials as it is used 
in the example above (see http://docs.aws.amazon.com/cli/latest/userguide/cli-chap-getting-started.html)

# ParameterStore on AWS

see: http://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-paramstore.html