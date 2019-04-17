using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OTP_WEBSERVER.Models;

namespace OTP_WEBSERVER.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationRepository _applicationRepository;

        public HomeController(
            IUserRepository userRepository,
            IApplicationRepository applicationRepository
            )
        {
            _userRepository = userRepository;
            _applicationRepository = applicationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var msg = await _userRepository.InitiateDb();
                ViewData["db_already_set"] = msg == UserRepository.database_already_set;
            }
            catch
            {

            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var user = await _userRepository.CheckPassword(dto.Username, dto.Password);
                if (user == null)
                    throw new Exception("Invalid Password");

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var userIdentity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal);

                return RedirectToAction("Admin");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            var userid = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var userBsonId = ObjectId.Parse(userid);
            var name = User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            ViewData["Username"] = name;

            var userApps = await _applicationRepository.GetUsersApplications(userBsonId);

            var model = new AdminDto
            {
                Applications = userApps
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ApplicationDetails(string id)
        {
            var userid = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var userBsonId = ObjectId.Parse(userid);
            var name = User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            ViewData["Username"] = name;

            Application model;
            if (string.IsNullOrWhiteSpace(id) || id == "0")
            {
                model = Application.GenerateApplication();
            }
            else
            {
                var bsonId = ObjectId.Parse(id);
                model = await _applicationRepository.Get(bsonId);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApplicationDetails(string id, Application model)
        {
            var bsonId = ObjectId.Parse(id);
            var userid = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var userBsonId = ObjectId.Parse(userid);
            var name = User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            ViewData["Username"] = name;

            var oldApp = await _applicationRepository.Get(bsonId);
            if (oldApp.User_Id != userBsonId)
            {
                ViewData["Message"] = "Access Denied! You are not  owner of this application";
                ViewData["Success"] = false;
            }
            else if (!TryValidateModel(model))
            {
                ViewData["Message"] = "Not Saved! Please solve validation errors in the form.";
                ViewData["Success"] = false;
            }
            else
            {
                ViewData["Message"] = "Saved";
                ViewData["Success"] = true;

                model.User_Id = userBsonId;
                model.Id = bsonId;

                if (model.Id == ObjectId.Empty)
                    await _applicationRepository.Add(model);
                else
                {
                    await _applicationRepository.Update(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteApplication(string id)
        {
            var userid = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var userBsonId = ObjectId.Parse(userid);
            var name = User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            ViewData["Username"] = name;

            if (await _applicationRepository.Delete(ObjectId.Parse(id)))
            {
                return RedirectToAction("Admin");
            }
            else
            {
                ViewData["Message"] = "Not Deleted! Error Occured";
                ViewData["Success"] = false;
                return RedirectToAction("ApplicationDetails", new { id });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
