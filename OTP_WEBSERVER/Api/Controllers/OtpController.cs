using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OTP_WEBSERVER.Models;
using OtpNet;

namespace OTP_WEBSERVER.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationRepository _applicationRepository;

        public OtpController(
            IUserRepository userRepository,
            IApplicationRepository applicationRepository
            )
        {
            _userRepository = userRepository;
            _applicationRepository = applicationRepository;
        }

        private Totp GetTotpFromApp(Application app, string extraKey = "")
        {
            var secretKeyBytes = Encoding.UTF8.GetBytes($"{app.SecretKey}{extraKey}");
            var totp = new Totp(secretKeyBytes, app.Step, app.HashMode, app.Size);
            return totp;
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

        [HttpGet("GetTotp")]
        public async Task<ActionResult<string>> GetTotp(string sharedKey, string userKey)
        {
            try
            {
                Disable_CertificateValidation();
                var app = await _applicationRepository.Get(sharedKey);
                if (app == null)
                    throw new KeyNotFoundException($"There is no application with the key {sharedKey}");

                var totpMachine = GetTotpFromApp(app, userKey);
                var result = totpMachine.ComputeTotp();
                return result;
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet("ValidateTotp")]
        public async Task<ActionResult<bool>> ValidateTotp(string totp, string sharedKey, string userKey)
        {
            try
            {
                Disable_CertificateValidation();
                var app = await _applicationRepository.Get(sharedKey);
                if (app == null)
                    throw new KeyNotFoundException($"There is no application with the key {sharedKey}");

                long timeStepMatched = 0;
                var totpMachine = GetTotpFromApp(app, userKey);                
                var result = totpMachine.VerifyTotp(totp, out timeStepMatched);
                return result;
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}