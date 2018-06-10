using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Reserveer.Models;
using Reserveer.Data;
using MySql.Data.MySqlClient;
using System;

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


    public IActionResult UpdateRoom(RoomInfo info)
    {
      using (MySqlConnection conn = new MySqlConnection())
      {
        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";

        conn.Open();
        String sql =
            "UPDATE rooms SET rooms.room_name = '" + info.RoomName + "', rooms.room_number = " + info.RoomNumber + ",  rooms.room_floor = " + info.RoomFloor + ", rooms.room_facilities = '" + info.RoomFacility + "', rooms.room_comment = '" + info.RoomComment + "' WHERE rooms.room_id = " + info.RoomID + "; ";
        MySqlCommand command = new MySqlCommand(sql, conn);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Index", "GroupsAdmin");
      }
    }

    public IActionResult AddRoomInfo(RoomInfo info)
    {
      using (MySqlConnection conn = new MySqlConnection())
      {
        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";

        conn.Open();
        String sql =
            "INSERT INTO rooms(room_name, room_number, group_id, room_floor, room_facilities,  available, room_comment) VALUES ('" + info.RoomName + "', " + info.RoomNumber + ", 1, " + info.RoomFloor + ", '" + info.RoomFacility + "', 0, '" + info.RoomComment + "'); ";
        MySqlCommand command = new MySqlCommand(sql, conn);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Index", "GroupsAdmin");
      }
    }

    public IActionResult DeleteReservation(string test)
    {
      using (MySqlConnection conn = new MySqlConnection())
      {
        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        //string test123 = "1";
        conn.Open();
        String sql =
          "UPDATE user SET user.availability = '1' WHERE user.user_id = '" + test + "';";
          MySqlCommand command = new MySqlCommand(sql, conn);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Index", "GroupsAdmin");
      }
    }

    public IActionResult DeactivateUser()
    {
      using (MySqlConnection conn = new MySqlConnection())
      {
        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        string test123 = "1";
        conn.Open();
        String sql =
          "DELETE FROM reservations WHERE reservations.id = " + test123 + ";";
        MySqlCommand command = new MySqlCommand(sql, conn);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Index", "GroupsAdmin");
      }
    }
  }
}

// GROEPID NOG AANPASSEN BIJ ADDROOM