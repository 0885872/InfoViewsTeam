using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Reserveer.Data;
using Reserveer.Models;

namespace Reserveer.Controllers
{
    public class HomeController : Controller
    {
        private readonly DutchContext _context;

        public string UserName;

        public HomeController(DutchContext context)
        {
            _context = context;
        }

  //      public IActionResult Index()
    //    {
   //         return View();
   //     }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(UserRegistration user)
        {
            Database db = new Database();
            string[] result = db.FindDuplicates(user);
            string domain = user.Mail.ToString();
            string[] domainArray = domain.Split("@");
            string domainExists = domainArray[1];
            string group_id = db.getDomainCheck(domainExists);

            if (group_id == "null")
            {
                ModelState.AddModelError("Mail", "There is no group allocated to this email. Contact your administrator for more information.");
                return View();
            }
            else
            {
                if (result[0] == "0")
                {
                    using (MySqlConnection conn = new MySqlConnection())
                    {
                        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";

                        conn.Open();
                        String sql =
                            "INSERT INTO user (group_id,user_name, user_mail, user_password, user_role, active) VALUES (" +
                            group_id + ",'" + user.Name + "','" + user.Mail + "','" + user.Password +
                            "', 'user', 0);";
                        MySqlCommand command = new MySqlCommand(sql, conn);
                        command.ExecuteNonQuery();
                        conn.Close();
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("Mail", "Email is already taken");
                    return View();
                }
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Index(User user)
        {
            if (ModelState.IsValid)
            {
                if (IsValid(user.user_mail, user.user_password))
                {
                        List<Claim> claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.user_mail),
                        };
                        var userIdentity =
                            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(principal);
                        UserName = principal.Identity.Name;

                        return RedirectToAction("Index", "Groups");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Login Data is incorrect");
                }
            return View();
        }

        public async Task <IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private bool IsValid(string email, string password)
        {
            bool isValid = false;
            var user = _context.user.FirstOrDefault(u => u.user_mail == email);

            if (user != null)
            {
                if (user.user_password == password)
                {
                    isValid = true;
                }
            }
            return isValid;
        }
    }
}
