using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyOtpDemoBankWeb.Models;

namespace MyOtpDemoBankWeb.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private ApplicationDbContext _context;
        private IOptions<OtpServerSettings> _otpOptions;
        private IHttpClientFactory _clientFactory;

        public LoginController(
            ApplicationDbContext context,
            IOptions<OtpServerSettings> otpOptions,
            IHttpClientFactory clientFactory)
        {
            _context = context;
            _otpOptions = otpOptions;
            _clientFactory = clientFactory;
        }

        //[Obsolete("Do not use this in Production code!!!", true)]
        static void Disable_CertificateValidation()
        {
            // Disabling certificate validation can expose you to a man-in-the-middle attack
            // which may allow your encrypted message to be read by an attacker
            // https://stackoverflow.com/a/14907718/740639
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                ) {
                    return true;
                };
        }

        [HttpGet("GetOtp")]
        public async Task<string> GetOtp(string username)
        {
            Disable_CertificateValidation();
            var url = $"{_otpOptions.Value.EndPoint}{_otpOptions.Value.Controller}{_otpOptions.Value.GetTOtpAction}?sharedKey={_otpOptions.Value.SharedKey}&userKey={username}";
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                //var result = await response.Content.ReadAsAsync<string>();
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
                return string.Empty;
        }

        [HttpGet("ValidateOtp")]
        public async Task<bool> ValidateOtp(string username, string totp)
        {
            Disable_CertificateValidation();
            var url = $"{_otpOptions.Value.EndPoint}{_otpOptions.Value.Controller}{_otpOptions.Value.ValidataTOtpAction}?sharedKey={_otpOptions.Value.SharedKey}&userKey={username}&totp={totp}";
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<bool>();
                if (result)
                {
                    var loginRequests = _context.LoginRequests.Where(x => x.IsValidated == false && x.Username == username);
                    foreach (var lr in loginRequests)
                    {                        
                        _context.Attach(lr);
                        _context.Entry(lr).Property(x => x.IsValidated).IsModified = true;
                        lr.IsValidated = true;
                    }
                    _context.SaveChanges();
                }
                return result;
            }
            else
                return false;
        }

        [HttpGet("IsAnyLoginRequest")]
        public bool IsAnyLoginRequest(string username)
        {
            Disable_CertificateValidation();
            var result = _context.LoginRequests.Any(x => x.IsValidated == false && x.Username == username);
            return result;
        }
    }
}