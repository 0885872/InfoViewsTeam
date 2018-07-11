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
using Microsoft.AspNetCore.Authorization;

namespace Reserveer.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private readonly DutchContext _context;
        public string UserName;
        public static string UserRole;
        public static int UserId;
        public string Username;
        public int active;
        private static Random random = new Random();

        public HomeController(DutchContext context) // Connects to database
        {
            _context = context;
        }

        // Redirects to Error page
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Index() // Populates Home->Index with data
        {
            {
                try
                {
                    // Checks to see if user is authenticated as an user or admin
                    if (User.Identity.IsAuthenticated)
                    {
                        if (User.IsInRole("user"))
                        {
                            string a = Request.Query["ReturnUrl"];
                            var request = HttpContext.Request;
                            var test = request.Path;
                            return RedirectToAction("Index", "Groups");
                        }

                        if (User.IsInRole("admin"))
                        {
                            return RedirectToAction("Index", "GroupsAdmin");
                        }
                    }

                    string mail = Request.Query["mail"];
                    string token = Request.Query["number"];

                    // Verifies the given usermail address and token
                    if (mail != null && token != null)
                    {
                        Database database = new Database();
                        bool verified = database.VerifyMail(mail, token);
                        ViewBag.verified = verified;
                    }
                    return View();
                }
                // Catches an exception error and redirect to homepage
                catch (Exception e)
                {
                    Debug.WriteLine("Index Exception login {0}", e);
                    return RedirectToAction("Error", "Home");
                    throw;
                }
            }

        }


        public IActionResult Registration() // Shows registration
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Registration(UserRegistration user) // Posts Registration Data
        {
            if (ModelState.IsValid)
            {
                //Adds simplecrypto for encryption password
                var crypto = new SimpleCrypto.PBKDF2();
                var encrypass = crypto.Compute(user.Password);

                Database db = new Database();
                string[] result = db.FindDuplicates(user);
                string domain = user.Mail.ToString();
                string[] domainArray = domain.Split("@");
                string domainExists = domainArray[1];
                string group_id = db.getDomainCheck(domainExists);

                {
                    try
                    {
                        if (group_id == "null") // Checks for existing group connected to email
                        {
                            ModelState.AddModelError("Mail",
                                "There is no group allocated to this email. Contact your administrator for more information.");
                            return View();
                        }
                        else
                        {
                            if (result[0] == "0")
                            {
                                using (MySqlConnection conn = new MySqlConnection())
                                {
                                    //Set user in database
                                    conn.ConnectionString =
                                        "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";

                                    conn.Open();
                                    String sql =
                                        "INSERT INTO user (group_id,user_name, user_mail, user_password, password_salt, user_role, active) VALUES (" +
                                        group_id + ",'" + user.Name + "','" + user.Mail + "','" + encrypass + "','" +
                                        crypto.Salt +
                                        "', 'user', 0);";
                                    MySqlCommand command = new MySqlCommand(sql, conn);
                                    command.ExecuteNonQuery();
                                    conn.Close();

                                    //get user_id from database
                                    conn.Open();
                                    String iDsql =
                                        "SELECT user_id from user where user_mail = '" + user.Mail + "';";
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
                                    // Verification key that is randomly generated
                                    string rrandom = new string(Enumerable.Repeat(chars, 15)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());

                                    // Saving randomly generated verification key in database
                                    string insertVerStr =
                                        "INSERT INTO registration_validation (user_id, registration_key) VALUES (" +
                                        res[0] + ", '" + rrandom + "');";
                                    MySqlCommand verSqlstring = new MySqlCommand(insertVerStr, conn);
                                    verSqlstring.ExecuteNonQuery();
                                    conn.Close();

                                    //Generating email and verification message
                                    MailMessage msg = new MailMessage();
                                    SmtpClient smtp = new SmtpClient();

                                    // Makes the message
                                    string verifyLink = "http://145.24.222.130/?mail=" + user.Mail + "&number=" + rrandom;
                                    msg.From = new MailAddress("Noreply@infoviews.drakonit.nl");
                                    msg.To.Add(user.Mail);
                                    msg.Subject = "E-mail verification";
                                    msg.Body = "Hi there, click the following link to activate your account: " + verifyLink;

                                    // Sends the message to the email
                                    var client = new SmtpClient("smtp.hro.nl", 25);
                                    client.Send(msg);
                                    TempData["verification_allowed"] = "true";
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("Mail", "Email is already taken");
                                return View();
                            }
                        }
                    }
                    // Catches exception error and redirects to error page
                    catch (Exception e)
                    {
                        Debug.WriteLine("Registration Exception {0}", e);
                        return RedirectToAction("Error", "Home");
                        throw;
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(User user) // Posts Index data
        {
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        // Looks if IsValid is true and user account is active
                        if (IsValid(user.user_mail, user.user_password) && active == 1)
                        {
                            // Saves the Username and Userrole in claims
                            List<Claim> claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, Username),
                                new Claim(ClaimTypes.Role, UserRole)
                            };
                            // Save the roles and name in userIdentity to make use of authentication
                            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            // Add Identity to make use off the build in protocols it haves
                            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                            // Signs in
                            await HttpContext.SignInAsync(principal);
                            // Gets the username
                            UserName = principal.Identity.Name;

                            // Looks at the role of the user 
                            if (principal.IsInRole("user"))
                            {
                                // Gets value from the ReturnUrl variable in the url at homepage
                                string URLRedirection = Request.Query["ReturnUrl"];

                                if (URLRedirection != null)
                                {
                                    string RoomIdURL = URLRedirection.Substring(URLRedirection.IndexOf("=") + 1);
                                    return RedirectToAction("Index", "Schedule", new { RoomId = RoomIdURL });
                                }
                                return RedirectToAction("Index", "Groups");
                            }

                            // Looks at the role of the user 
                            if (principal.IsInRole("admin"))
                            {
                                return RedirectToAction("Index", "GroupsAdmin");
                            }
                        }

                        // Looks if it is not an active user
                        if (IsValid(user.user_mail, user.user_password) && active == 0)
                        {
                            ModelState.AddModelError("", "User Account is not activated");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Login Data is incorrect");
                        }
                    }
                    return View();
                }
                // If it catches an exception error it redirects to an error page
                catch (Exception e)
                {
                    Debug.WriteLine("Index login post Exception {0}", e);
                    return RedirectToAction("Error", "Home");
                    throw;
                }
            }
        }

        public async Task<IActionResult> LogOut() // Redirects and logs out user
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private bool IsValid(string email, string password) // Check for validation of the login input
        {
            {
                try
                {
                    //Add simpleCrypto to encrypt
                    var crypto = new SimpleCrypto.PBKDF2();
                    bool isValid = false;
                    // Check to see if email is correct
                    var user = _context.user.FirstOrDefault(u => u.user_mail == email);

                    //Check to see if it is not an active user
                    if (user != null && user.active != 0)
                    {
                        UserRole = user.user_role;
                        UserId = user.user_id;
                        Username = user.user_name;
                        active = user.active;

                        // encrypts input password and makes a length check of the password in database to compare it with the encrypted input password
                        string toCompare = crypto.Compute(password, user.password_salt);
                        int amount = user.user_password.Length;
                        string passwordSaltFixed = toCompare.Substring(0, amount);
                        string compareString = user.user_password.Substring(0, amount);

                        if (passwordSaltFixed == compareString)
                        {
                            isValid = true;
                        }
                    }

                    // Check to see if it is an active user
                    else if (user != null && user.active == 0)
                    {
                        string toCompare = crypto.Compute(password, user.password_salt);
                        int amount = user.user_password.Length;
                        string passwordSaltFixed = toCompare.Substring(0, amount);
                        string compareString = user.user_password.Substring(0, amount);

                        if (passwordSaltFixed == compareString)
                        {
                            isValid = true;
                        }
                    }
                    return isValid;
                }
                // If it catches an exception error it redirects to an error page
                catch (Exception e)
                {
                    Debug.WriteLine("Isvalid bool login Exception: {0}", e);
                    throw;
                }
            }
        }
    }
}
