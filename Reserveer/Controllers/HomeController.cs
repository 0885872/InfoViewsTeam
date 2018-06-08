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
using System.Net.Mail;

namespace Reserveer.Controllers
{
    public class HomeController : Controller
    {
        private readonly DutchContext _context;
        public string UserName;
        private static Random random = new Random();

        public HomeController(DutchContext context)
        {
            _context = context;
        }

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

            var crypto = new SimpleCrypto.PBKDF2();
            var encrypass = crypto.Compute(user.Password);

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
                        //Set user in database
                        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";

                        conn.Open();
                        String sql =
                            "INSERT INTO user (group_id,user_name, user_mail, user_password, password_salt, user_role, active) VALUES (" +
                            group_id + ",'" + user.Name + "','" + user.Mail + "','" + encrypass + "','" + crypto.Salt +
                            "', 'user', 0);";
                        MySqlCommand command = new MySqlCommand(sql, conn);
                        command.ExecuteNonQuery();
                        conn.Close();

                        //get user_id from database
                        conn.Open();
                        String iDsql =
                            "SELECT user_id from user where user_mail = '" + user.Mail + "';" ;
                        MySqlCommand uIdcmd = new MySqlCommand(iDsql, conn);

                        string[] res = new string[1];
                        using (MySqlDataReader reader = uIdcmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res[0] = reader["user_id"].ToString();
                            }
                        }
                        conn.Close();

                        //Set verification key in database
                        conn.Open();
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        string rrandom = new string(Enumerable.Repeat(chars, 15)
                          .Select(s => s[random.Next(s.Length)]).ToArray());

                        string insertVerStr = "INSERT INTO registration_validation (user_id, registration_key) VALUES (" + res[0] + ", '" + rrandom + "');";
                        MySqlCommand verSqlstring = new MySqlCommand(insertVerStr, conn);
                        verSqlstring.ExecuteNonQuery();
                        conn.Close();

                        MailMessage msg = new MailMessage();
                        SmtpClient smtp = new SmtpClient();

                        string verifyLink = "http://infoviews.drakonit.nl/Register/?mail=" + user.Mail + "&number=" + rrandom;
                        msg.From = new MailAddress("Noreply@infoviews.drakonit.nl");
                        msg.To.Add(user.Mail);
                        msg.Subject = "E-mail verification";
                        msg.Body = "Hi there, click the following link to activate your account: " + verifyLink;

                        var client = new SmtpClient("smtp.hro.nl", 25);
                        client.Send(msg);

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
                            new Claim(ClaimTypes.Name, user.user_mail)
                        };
                        var userIdentity =
                            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(principal);
                        UserName = principal.Identity.Name;

                        return RedirectToAction("Index", "Groups");
                }
                else
                {
                    ModelState.AddModelError("", "Login Data is incorrect");
                }
            }
            return View();
        }

        private bool IsValid(string email, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool isValid = false;
            var user = _context.user.FirstOrDefault(u => u.user_mail == email);
            string toCompare = crypto.Compute(password, user.password_salt);
            var groupid = user.group_id;
            int amount = user.user_password.Length;
            string passwordSaltFixed = toCompare.Substring(0, amount);
            string compareString = user.user_password.Substring(0, amount);

            if (user != null)
            {
                if (passwordSaltFixed == compareString)
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
