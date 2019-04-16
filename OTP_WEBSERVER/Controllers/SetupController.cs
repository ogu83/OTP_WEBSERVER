using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OTP_WEBSERVER.Models;

namespace OTP_WEBSERVER.Controllers
{
    public class SetupController : Controller
    {
        private readonly IUserRepository _userRepository;

        public SetupController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var msg = string.Empty;
            try
            {
                msg =  await _userRepository.InitiateDb();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            
            ViewData["Message"] = msg;
            return View();
        }
    }
}
