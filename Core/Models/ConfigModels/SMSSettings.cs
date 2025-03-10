namespace Core.Models.ConfigModels
{
    public class SMSSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string TokenUrl { get; set; }
        public string SMSUrl { get; set; }
        public string BulkSMSUrl { get; set; }
    }
}
