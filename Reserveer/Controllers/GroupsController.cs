using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
namespace Reserveer.Controllers
{
    [Authorize(Roles = "user")]
    public class GroupsController : Controller
    {
    // 
    // GET: /Groups/

        public IActionResult Index(int numTimes = 1)
        {
            Database db = new Database();
            List<string[]> results = db.getUserGroup();
            var json = JsonConvert.SerializeObject(results);
            ViewData["results"] = json;

            ViewData["NumTimes"] = numTimes;
            return View();
        }

        // 
        // GET: /Groups/Welcome/  
        public IActionResult Rooms(string name, int numTimes = 7)
        {

            Database db = new Database();
            List<string[]> results = db.getGroupRooms();
            var json = JsonConvert.SerializeObject(results);
            ViewData["results"] = json;

            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;

            return View();
        }

            





    }
}