using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reserveer.Models;
using Newtonsoft.Json;

namespace Reserveer.Controllers
{
  public class ScheduleController : Controller
  {
        //Reservaties ophalen
        [HttpGet]
        public ActionResult Index(int numTimes = 1)
        {
            ViewData["NumTimes"] = numTimes;
            Database db = new Database();
            List<string[]> results = db.getReservations();
            var json = JsonConvert.SerializeObject(results);
            ViewData["results"] = json;
            return View();
        }

        //Reservaties posten
        [HttpPost]
        public ActionResult SetReservation([FromBody]ReservationModel reservation)
        {
            ReservationModel reservations = new ReservationModel
            {
                title = reservation.title,
                start = reservation.start,
                end = reservation.end

            };
            Database db = new Database();
            db.setReservations(reservations);
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