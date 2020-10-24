namespace SelfHostApi.Models
{
    public class ConnectionSettings : BaseAbstractModel
    {
        public string EndPoint { get; set; }
        public int Timeout { get; set; }
        public bool IsRetry { get; set; }
        public int Attempts { get; set; }
        public override ETypes Type => ETypes.ConnectionSettings;
    }
}