
# ParameterStoreConfigurationProvider [![NuGet](https://img.shields.io/nuget/v/ParameterStoreConfigurationProvider.svg)](https://www.nuget.org/packages/ParameterStoreConfigurationProvider/)

Enrich your configuration with plain text or secure settings from AWS ParameterStore.

## Example

Example configuration for an ASP.Net Core project:

        public static IWebHost BuildWebHost(string[] args)
        {

            return WebHost.CreateDefaultBuilder(args)
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

# Alternatively

You can also use parameterStoreConfig.UseDefaultCredentials = true; to let AWS handle this.

# Finding your way in the solution

In /src you find two projects, one example-api and the src for the nuget package itself
In /test you find the unit tests

To use the provided samples you have to will have to setup 3 parameters in the ParameterStore

/somenamespace/somekey
/somenamespace/someotherkey
/somenamespace/somesecurekey => This needs to be set up as a secure string, which requires a KMS-Encryption key

You will also have set up and save a local default aws profile on the computer if you want to use StoredProfileAWSCredentials as it is used in the example above (see http://docs.aws.amazon.com/cli/latest/userguide/cli-chap-getting-started.html)

# ParameterStore on AWS

For reference see the [Parameter Store documentation on AWS](https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-parameter-store.html)
