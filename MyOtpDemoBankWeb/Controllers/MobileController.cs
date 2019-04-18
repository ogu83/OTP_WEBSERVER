using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyOtpDemoBankWeb.Models;

namespace MyOtpDemoBankWeb.Controllers
{
    public class MobileController : Controller
    {
        private ApplicationDbContext _context;

        public MobileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Success"] = false;
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            var myUser = _context.Users.FirstOrDefault(x => x.Username == user.Username && x.Password == user.Password);
            if (myUser == null)
            {
                ViewData["Message"] = "Access Denied!";
                ViewData["Success"] = false;
                return View(user);
            }
            else
            {
                //ViewData["Message"] = "Access Granted";
                //ViewData["Success"] = true;
                return RedirectToAction("OtpPage", "Mobile", new { username = user.Username });
            }
        }

        [HttpGet]
        public IActionResult OtpPage(string username)
        {
            ViewData["username"] = username;
            return View();
        }
    }
}