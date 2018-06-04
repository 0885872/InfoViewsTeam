using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace InfoView.Controllers
{
  public class ScheduleController : Controller
  {
    // 
    // GET: /Groups/

    public IActionResult Index()
    {
      return View();
    }

    // 
    // GET: /Groups/Welcome/  
    public IActionResult Rooms(string name, int numTimes = 1)
    {
      ViewData["Message"] = "Hello " + name;
      ViewData["NumTimes"] = numTimes;

      return View();
    }



  }
}