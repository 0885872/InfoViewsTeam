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
            string room = Request.Query["RoomId"];
            ViewData["NumTimes"] = numTimes;
            Database db = new Database();
            List<string[]> results = db.getReservations(room);
            var json = JsonConvert.SerializeObject(results);
            ViewData["results"] = json;
            return View();
        }

        //Reservaties posten
        [HttpPost]
        public ActionResult SetReservation([FromBody]ReservationModel reservation)
        {
            int userid = HomeController.UserId;
            string room = Request.Query["RoomId"];
            ReservationModel reservations = new ReservationModel
            {
                title = reservation.title,
                start = reservation.start,
                end = reservation.end,
                roomid = room


            };
            Database db = new Database();
            db.setReservations(reservations);

            MailMessage msg = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            msg.From = new MailAddress("Noreply@infoviews.drakonit.nl");
            msg.To.Add("0885872@hr.nl");
            msg.Subject = "Confirmation of reservation";
            msg.Body = "Hi there, We would like to inform you: We've saved your reservation! start: " + reservation.start + ", end: " + reservation.end + ", room: " + reservation.roomid + ". Thanks for using InfoViews!";

            var client = new SmtpClient("smtp.hro.nl", 25);
            client.Send(msg);

            return View("Index");
        }
        //// 
        //// GET: /Groups/

        //public IActionResult Index()
        //{
        //    ViewData["NumTimes"] = numTimes;
        //    return View();
        //}

        //// 
        //// GET: /Groups/Welcome/  
        //public IActionResult Rooms(string name, int numTimes = 1)
        //{
        //  ViewData["Message"] = "Hello " + name;
        //  ViewData["NumTimes"] = numTimes;

        //  return View();
        //}



    }
}