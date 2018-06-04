using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public HomeController(DutchContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                if (IsValid(user.user_mail, user.user_password))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login Data is incorrect");
                }
            }
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(UserRegistration user)
        {
            int groupid = 1;
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                conn.Open();
                String sql =
                    "INSERT INTO user (group_id,user_name, user_mail, user_password, user_role, active) VALUES (" +
                    groupid + ",'" + user.Name + "','" + user.Mail + "','" + user.Password +
                    "', 'user', 0);";
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
            return View();
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

        public ActionResult LogOut()
        {
            SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
