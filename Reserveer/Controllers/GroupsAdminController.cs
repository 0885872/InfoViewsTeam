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
        string roomid = Request.Query["roomid"];
        Database db = new Database();
        List<string[]> results = db.getRoomProfileInfo(roomid);
        List<string[]> results2 = db.getRoomReservation();
        List<string[]> results3 = db.getRoomSensors(roomid);
        var json3 = JsonConvert.SerializeObject(results3);
        var json2 = JsonConvert.SerializeObject(results2);
        var json = JsonConvert.SerializeObject(results);
        ViewData["results"] = json;
        ViewData["results2"] = json2;
        ViewData["results3"] = json3;
        return View();
    }

    [ValidateAntiForgeryToken]
    public IActionResult UpdateRoom(RoomInfo info, GroupInfo test)
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
        return RedirectToAction("Profile", "GroupsAdmin", test.GroupID);
        }
    }

    [ValidateAntiForgeryToken]
    public IActionResult UpdateRoomSensor(RoomInfo info, GroupInfo test)
    {
        using (MySqlConnection conn = new MySqlConnection())
        {
            conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            conn.Open();
            String sql = "UPDATE sensors SET assigned = 1 WHERE sensor_id = "+info.SensorID+" ";
            MySqlCommand command = new MySqlCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
            conn.Open();
            String sql2 = "INSERT INTO `rooms_has_sensors` VALUES('1','" + info.SensorID + "') ";
            MySqlCommand command2 = new MySqlCommand(sql2, conn);
            command2.ExecuteNonQuery();
            conn.Close();
            
            return RedirectToAction("Profile", "GroupsAdmin", test.GroupID);
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

    [ValidateAntiForgeryToken]
    public IActionResult UpdateGroupName(GroupInfo info)
    {
        using (MySqlConnection conn = new MySqlConnection())
        {
            conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            //string test123 = "1";
            conn.Open();
            String sql =
                "UPDATE `group` SET group.group_name = " + info.GroupName + " WHERE group.group_id = '" + info.GroupID + "'";
            MySqlCommand command = new MySqlCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Profile", "GroupsAdmin", info.GroupID);
        }
    }

    public IActionResult DeactivateUser(string userId, string groupId)
    {
        using (MySqlConnection conn = new MySqlConnection())
        {
            conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            //string test123 = "1";
            conn.Open();
            String sql =
              "UPDATE user SET user.active = '1' WHERE user.user_id = '" + userId + "';";
              MySqlCommand command = new MySqlCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Profile", "GroupsAdmin", groupId);
        }
    }

    public IActionResult DeleteReservation(string reservationId, string groupId)
    {
        using (MySqlConnection conn = new MySqlConnection())
        {
        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        //string test123 = "1";
        conn.Open();
        String sql =
          "UPDATE reservations SET reservations.valid = '1'  WHERE reservations.reservation_id = " + reservationId + ";";
        MySqlCommand command = new MySqlCommand(sql, conn);
        command.ExecuteNonQuery();
        conn.Close();
        return RedirectToAction("Profile", "GroupsAdmin", groupId);
        }
    }


    }
}

// GROEPID NOG AANPASSEN BIJ ADDROOM