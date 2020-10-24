using System;

namespace SelfHostApi.Models
{
    public class ConnectionSettings : BaseAbstractModel
    {
        public string EndPoint { get; set; }
        public int Timeout { get; set; }
        public bool IsRetry { get; set; }
        public int Attempts { get; set; }
        public override ETypes Type => ETypes.ConnectionSettings;

        private bool Equals(ConnectionSettings otherSettings)
        {
            return string.Equals(EndPoint, otherSettings.EndPoint, StringComparison.Ordinal) &&
                   Timeout == otherSettings.Timeout && IsRetry == otherSettings.IsRetry &&
                   Attempts == otherSettings.Attempts;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is ConnectionSettings settings && Equals(settings);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Type;
                hashCode = (hashCode * 397) ^ Timeout.GetHashCode();
                hashCode = (hashCode * 397) ^ Attempts.GetHashCode();
                hashCode = (hashCode * 397) ^ IsRetry.GetHashCode();
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(EndPoint) ? EndPoint.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}