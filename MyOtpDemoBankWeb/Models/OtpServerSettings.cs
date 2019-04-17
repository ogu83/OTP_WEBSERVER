namespace MyOtpDemoBankWeb.Models
{
    public class OtpServerSettings
    {
        public string EndPoint { get; set; }
        public string Controller { get; set; }
        public string GetTOtpAction { get; set; }
        public string ValidataTOtpAction { get; set; }
        public string SharedKey { get; set; }
    }
}