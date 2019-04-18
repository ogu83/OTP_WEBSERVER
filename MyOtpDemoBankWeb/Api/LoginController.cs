using System.Linq;
using System.Net.Http;
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

        [HttpGet("GetOtp")]
        public async Task<string> GetOtp(string username)
        {
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
            var result = _context.LoginRequests.Any(x => x.IsValidated == false && x.Username == username);
            return result;
        }
    }
}