using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace InfoView.Controllers
{
  public class GroupsAdminController : Controller
  {
    // 
    // GET: /Groups/
    public IActionResult Index()
    {
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