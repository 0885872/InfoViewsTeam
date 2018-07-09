using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Reserveer.Models;

namespace Reserveer.Controllers
{
    [Authorize(Roles = "user")]
    public class GroupsController : Controller
    {
        // 
        // GET: /Groups/

        public IActionResult Index() // Fills The Groups->Index view with data
        {
            {
                try
                {
                    Database db = new Database();
                    List<string[]> results = db.getUserGroup();
                    var json = JsonConvert.SerializeObject(results);
                    ViewData["results"] = json;

                    List<string[]> UserInfoList = db.getUserInfo();
                    var UserInfojson = JsonConvert.SerializeObject(UserInfoList);
                    ViewData["UserInfoResults"] = UserInfojson;

                    List<string[]> UserReservationsList = db.getUserReservations();
                    var UserReservationsjson = JsonConvert.SerializeObject(UserReservationsList);
                    ViewData["UserReservationsResults"] = UserReservationsjson;

                    return View();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Index Groups Exception: {0}", e);
                    return RedirectToAction("Error", "Home");
                    throw;
                }
            }

        }

        // 
        // GET: /Groups/Welcome/  
        public IActionResult Rooms(string name) // Fills the Groups->Rooms with data
        {
            {
                try
                {
                    Database db = new Database();
                    List<string[]> results = db.getGroupRooms();
                    var json = JsonConvert.SerializeObject(results);
                    ViewData["results"] = json;

                    return View();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Rooms Exception: {0}", e);
                    return RedirectToAction("Error", "Home");
                    throw;
                }
            }

        }

        public IActionResult DeleteReservation(string reservationId, string groupId) // Deleted specified reservation
        {
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection())
                    {
                        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                        conn.Open();
                        String sql =
                            "UPDATE reservations SET reservations.valid = '0'  WHERE reservations.reservation_id = " + reservationId + ";";
                        MySqlCommand command = new MySqlCommand(sql, conn);
                        command.ExecuteNonQuery();
                        conn.Close();
                        return RedirectToAction("Index", "Home", groupId);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("DeleteReservation Exception: {0}", e);
                    return RedirectToAction("Error", "Home");
                    throw;
                }
            }

        }

        [HttpPost]
        public IActionResult UpdateUser(UpdateUserModel user) // Updates room info
    {
      try
      {
        if (ModelState.IsValid)
        {
          var crypto = new SimpleCrypto.PBKDF2();
          var encrypass = crypto.Compute(user.Passwordd);
          using (MySqlConnection conn = new MySqlConnection())
          {
            conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            conn.Open();
            String sql =
                "UPDATE user SET user_name = '" + user.Namee + "', user_password = '" + encrypass + "', password_salt = '" + crypto.Salt + "' WHERE user_id = " + HomeController.UserId + ";";
            MySqlCommand command = new MySqlCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Index", "Groups", HomeController.UserId);
          }
        }
        else
        {
          ModelState.AddModelError("", "Error bro");
        }
        return View("Index");
      }
      catch (Exception e)
      {
        Debug.WriteLine("Updateroom Exception: {0}", e);
        return RedirectToAction("Error", "Home");
        throw;
      }
    }
  }
}
