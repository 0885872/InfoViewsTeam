using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Newtonsoft.Json;

namespace Reserveer.Controllers
{
  public class GroupsAdminController : Controller
  {
    // 
    // GET: /Groups/
    public IActionResult Index(int numTimes = 1)
    {
      Database db = new Database();
      List<string[]> results = db.getGroupAdmin();
      var json = JsonConvert.SerializeObject(results);
      ViewData["results"] = json;
      ViewData["NumTimes"] = numTimes;
      return View();
    }

    // 
    // GET: /Groups/Welcome/  
    public IActionResult AddGroup()
    {

      return View();
    }
    public IActionResult AddRoom()
    {

        return View();
    }

        public IActionResult Profile()
    {
      return View();
    }

    public IActionResult RoomProfile()
    {
      return View();
    }



  }
}