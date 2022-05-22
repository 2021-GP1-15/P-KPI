using IP_KPI.Data;
using IP_KPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace IP_KPI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly db_a7baa5_ipkpiContext _db;

        public AccountController(ILogger<AccountController> logger, db_a7baa5_ipkpiContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Login(String returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> validate(String username, String password, String returnUrl)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var lastPass= "";
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                lastPass =hashedInputStringBuilder.ToString();
            }
            //check user table
            var User = _db.Users.FirstOrDefault(x => x.UserId == username && x.Password == lastPass);
            if (User != null)
                {
                    //User Exist -> true >>> Add Cookie
                    //Cookie Stuff
                    var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, User.UserId));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, User.UserId));
                claims.Add(new Claim(ClaimTypes.Role, User.Privilege));
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    if (returnUrl == null)
                    {
                    if (User.Privilege != "Admin")
                        returnUrl = "/Home/Homepage";
                    else
                        returnUrl = "/Admin/Index";
                    }
                    ViewData["returnUrl"] = returnUrl;

                    return Redirect(returnUrl);
                }
            
            // SSO -> flase
            TempData["Error"] = "البريد الالكتروني او كلمة المرور غير صحيحة";
            return View("Login");
        }

        public IActionResult AccessDenied()
        {
            TempData["Error"] = "غير مصرح لك بالدخول لهذه الصفحة، الرجاء تسجيل الدخول من جديد";
            return View("Login");
        }
        public async Task<IActionResult> Logout()
        {
            //confirmation method
            await HttpContext.SignOutAsync();
            return Redirect("Login");
        }

        public IActionResult SetPrivilege(String privilegeType, String privilegeRes, String userId)
        {
            var Privilege = new PrivilegeRequest(privilegeType, privilegeRes, userId);
            _db.Add(Privilege);
            _db.SaveChanges();
            return Json(true);
        }




    }
}
