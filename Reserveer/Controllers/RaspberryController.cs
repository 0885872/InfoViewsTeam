using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Reserveer.Controllers
{
    public class RaspberryController : Controller
    {
        //Reservaties ophalen
        [HttpGet]
        public ActionResult Index()
        {
            {
                try
                {
                    string room = Request.Query["roomid"];
                    Database db = new Database();
                    List<string[]> results = db.getReservations(room);
                    var json = JsonConvert.SerializeObject(results);
                    ViewData["results"] = json;
                    string[] temperatureData = db.getLatestTemperature(room);
                    var jsonTemp = JsonConvert.SerializeObject(temperatureData);
                    ViewData["temp"] = jsonTemp;
                    return View();

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Index raspberry Exception: {0}", e);
                    return RedirectToAction("Error", "Home");
                    throw;
                }}
        }
    }
}