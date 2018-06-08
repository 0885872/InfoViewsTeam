using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Reserveer.Controllers
{
    [Authorize(Roles = "admin")]
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
        Database db = new Database();
        List<string[]> results = db.getGroupAdmin();
        List<string[]> results2 = db.getGroupRooms();
        List<string[]> results3 = db.getGroupUser();
        List<string[]> results4 = db.getGroupRoomReservation();
        var json = JsonConvert.SerializeObject(results);
        var json2 = JsonConvert.SerializeObject(results2);
        var json3 = JsonConvert.SerializeObject(results3);
        var json4 = JsonConvert.SerializeObject(results4);
        ViewData["results"] = json;
        ViewData["results2"] = json2;
        ViewData["results3"] = json3;
        ViewData["results4"] = json4;
        return View();
    }

    public IActionResult RoomProfile()
    {
        Database db = new Database();
        List<string[]> results = db.getRoomProfileInfo();
        List<string[]> results2 = db.getRoomReservation();
        var json2 = JsonConvert.SerializeObject(results2);
        var json = JsonConvert.SerializeObject(results);
        ViewData["results"] = json;
        ViewData["results2"] = json2;
        return View();
    }



  }
}