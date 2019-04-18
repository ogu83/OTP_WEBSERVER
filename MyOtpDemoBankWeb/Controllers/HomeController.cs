using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyOtpDemoBankWeb.Models;

namespace MyOtpDemoBankWeb.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            initDb();
        }

        private void initDb()
        {
            if (!_context.Users.Any())
            {
                _context.Users.Add(new User
                {
                    Username = "brucewayne",
                    Password = "batman"
                });
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Success"] = false;
            return View(new User());
        }

        [HttpGet]
        public IActionResult Account()
        {
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
            }
            else
            {
                _context.LoginRequests.Add(new LoginRequest
                {
                    Username = myUser.Username,
                    IsValidated = false
                });
                _context.SaveChanges();

                ViewData["Message"] = "Waiting for otp confirmation!";
                ViewData["Success"] = true;
            }
            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}