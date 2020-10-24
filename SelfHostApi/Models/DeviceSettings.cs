using System;

namespace SelfHostApi.Models
{
    public class DeviceSettings : BaseAbstractModel
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public bool CanUseEthernet { get; set; }
        public string Name { get; set; }
        public override ETypes Type => ETypes.DeviceSettings;

        private bool Equals(DeviceSettings otherSettings)
        {
            return string.Equals(IP, otherSettings.IP, StringComparison.Ordinal) &&
                   Port == otherSettings.Port && CanUseEthernet == otherSettings.CanUseEthernet &&
                   string.Equals(Name, otherSettings.Name, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is DeviceSettings settings && Equals(settings);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Type;
                hashCode = (hashCode * 397) ^ CanUseEthernet.GetHashCode();
                hashCode = (hashCode * 397) ^ Port.GetHashCode();
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(IP) ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}