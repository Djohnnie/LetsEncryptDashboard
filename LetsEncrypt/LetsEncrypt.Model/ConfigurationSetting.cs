using LetsEncrypt.Model.Base;

namespace LetsEncrypt.Model
{
    public class ConfigurationSetting : ModelBase
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}