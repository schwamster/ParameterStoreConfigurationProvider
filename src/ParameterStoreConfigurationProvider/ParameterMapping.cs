namespace ParameterStoreConfigurationProvider
{
    public class ParameterMapping
    {
        public string AwsName { get; set; }
        public string SettingName { get; set; }
        public string Default { get; set; } = null;
        public bool Optional { get; set; } = false;
    }
}
