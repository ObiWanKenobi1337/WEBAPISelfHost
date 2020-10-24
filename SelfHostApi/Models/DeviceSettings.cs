namespace SelfHostApi.Models
{
    public class DeviceSettings : BaseAbstractModel
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public bool CanUseEthernet { get; set; }
        public string Name { get; set; }
        public override ETypes Type => ETypes.DeviceSettings;
    }
}