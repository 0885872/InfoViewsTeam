using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserveer.Models;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Reserveer.Controllers
{
    [Authorize(Roles = "admin,user")]
  public class ScheduleController : Controller
  {
        //Reservaties ophalen
        [HttpGet]
        public ActionResult Index(int numTimes = 1)
        {
            int userid = HomeController.UserId;
            string room = Request.Query["roomid"];
            ViewData["NumTimes"] = numTimes;
            Database db = new Database();
            List<string[]> results = db.getReservations(room);
            var json = JsonConvert.SerializeObject(results);
            ViewData["results"] = json;
            return View();
        }
    }
}