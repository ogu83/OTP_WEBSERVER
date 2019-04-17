namespace MyOtpDemoBankWeb.Models
{
    public class LoginRequest
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public bool IsValidated { get; set; }
    }
}