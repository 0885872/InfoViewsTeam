using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Reserveer.Models;
using Reserveer.Data;
using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;

namespace Reserveer.Controllers
{
    [Authorize(Roles = "admin")]
    public class GroupsAdminController : Controller
    {

        // 
        // GET: /Groups/
        public IActionResult Index()
        {
            try
            {
                Database db = new Database();
                // Creates list of string array for the result of getGroupAdmin returned by database
                List<string[]> results = db.getGroupAdmin();
                // Converts this list to a json results string
                var json = JsonConvert.SerializeObject(results);
                // Hands over json data to the view
                ViewData["results"] = json;
                return View();
            }
            // If an exception gets caught and an error occurs
            catch (Exception e)
            {
                Debug.WriteLine("Index Groupsadmin Exception: {0}", e);
                // Redirects the user to the error page
                return RedirectToAction("Error", "Home");
                throw;
            }
        }

        // 
        // GET: /Groups/Welcome/  
        public IActionResult AddRoom()
        {
            return View(); // Creates a new page to fill in data for a new room
        }

        public IActionResult Profile()
        {
            try
            {
                Database db = new Database();
                // Creates list of string array for the results returned by database.cs
                List<string[]> results = db.getGroupAdmin();
                List<string[]> results2 = db.getGroupRooms();
                List<string[]> results3 = db.getGroupUser();
                List<string[]> results4 = db.getGroupRoomReservation();
                // Converts these string array to json results string
                var json = JsonConvert.SerializeObject(results);
                var json2 = JsonConvert.SerializeObject(results2);
                var json3 = JsonConvert.SerializeObject(results3);
                var json4 = JsonConvert.SerializeObject(results4);
                // Hands over json data to the views
                ViewData["results"] = json;
                ViewData["results2"] = json2;
                ViewData["results3"] = json3;
                ViewData["results4"] = json4;
                return View();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Profile Exception: {0}", e);
                // Redirects to error page
                return RedirectToAction("Error", "Home");
                throw;
            }
        }
        //get Roomprofile
        public IActionResult RoomProfile()
        {
            try
            {
                string roomid = Request.Query["roomid"];
                Database db = new Database();
                // Creates list of string array for the results returned by database.cs
                List<string[]> results = db.getRoomProfileInfo(roomid);
                List<string[]> results2 = db.getRoomReservation(roomid);
                List<string[]> results3 = db.getCurrentRoomSensors(roomid);
                // If there is a sensor connected to the current room
                if (results3.Count == 1)
                {
                    string[] temperatureData = db.getLatestTemperature(roomid);
                    var jsonTemp = JsonConvert.SerializeObject(temperatureData);
                    ViewData["temp"] = jsonTemp;
                }
                // If there is not a sensor connected to the current room
                else
                {
                    ViewData["temp"] = "[]";
                }
                // Creates list of string array for the result of getAvailableRoomSensors for the current room returned by database
                List<string[]> results4 = db.getAvaibleRoomSensors(roomid);
                // Converts all string array to json results string
                var json4 = JsonConvert.SerializeObject(results4);
                var json3 = JsonConvert.SerializeObject(results3);
                var json2 = JsonConvert.SerializeObject(results2);
                var json = JsonConvert.SerializeObject(results);
                // Hands over json data to the views
                ViewData["results"] = json;
                ViewData["results2"] = json2;
                ViewData["results3"] = json3;
                ViewData["results4"] = json4;
                return View();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Roomprofile Exception: {0}", e);
                // Redirect to error page
                return RedirectToAction("Error", "Home");
                throw;
            }


        }
        //this function updates the room with the newly send info from roomprofile form
        [ValidateAntiForgeryToken]
        public IActionResult UpdateRoom(RoomInfo info, GroupInfo grp) // Updates room info
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection())
                {
                    // The connectionstring with the connection info to login to the server
                    conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                    conn.Open();
                    // Create sql string to Update the room with the filled in information
                    String sql =
                        "UPDATE rooms SET rooms.room_name = '" + info.RoomName + "', rooms.room_number = " + info.RoomNumber
                        + ",  rooms.room_floor = " + info.RoomFloor + ", rooms.room_facilities = '" + info.RoomFacility + "', rooms.room_comment = '"
                        + info.RoomComment + "' WHERE rooms.room_id = " + info.RoomID + "; ";
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                    // Close the connection after executing the query
                    conn.Close();
                    // Redirect to the GroupsAdmin/Profile page, with the GroupID of the current user
                    return RedirectToAction("Profile", "GroupsAdmin", grp.GroupID);
                }
            }
            catch (Exception e) // Caches any exceptions that would cause an error
            {
                Debug.WriteLine("UpdateRoom Exception: {0}", e);
                // Redirects to error page
                return RedirectToAction("Error", "Home");
                throw;
            }
        }

        //this function assingns or updates the rooms sensor
        [ValidateAntiForgeryToken]
        public IActionResult UpdateRoomSensor(RoomInfo info, GroupInfo test) // Updates the sensor assigned to the room
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection())
                {
                    // The connectionstring with the connection info to login to the server
                    conn.ConnectionString =
                    "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";

                    //if current sensor id is not assigned(New) and selected sensor id is default(nothing) do nothing
                    //if current sensor id and the newly changed sensor id is the same do nothing
                    if (info.SensorID == "Default" && info.CurrentSensorID == "New" || info.SensorID == info.CurrentSensorID)
                    {

                    }
                    //if a room has a sensor and you want
                    //to assign no sensor id(selected default) for a room
                    else if (info.SensorID == "Default" && info.CurrentSensorID != "New")
                    {
                        conn.Open();
                        //delete a record in database that says the current sensorid is connected to the room
                        String sql = "DELETE FROM `rooms_has_sensors` WHERE `rooms_has_sensors`.`sensor_id` = " +
                                     info.CurrentSensorID + "";
                        MySqlCommand command = new MySqlCommand(sql, conn);
                        command.ExecuteNonQuery();
                        conn.Close();
                        conn.Open();
                        //current sensor id set avaibility to 0 so it can be assinged to other rooms in the group
                        String sql2 = "UPDATE sensors SET assigned = 0 WHERE sensor_id = " + info.CurrentSensorID +
                                      " ";
                        MySqlCommand command2 = new MySqlCommand(sql2, conn);
                        command2.ExecuteNonQuery();
                        conn.Close();
                    }
                    //if current sensor id is assigned and selected sensor id is a new one
                    else if (info.SensorID != "Default" && info.CurrentSensorID != "New")
                    {
                        conn.Open();
                        //delete a record in database that says the current sensorid is connected to the room
                        String sql = "DELETE FROM `rooms_has_sensors` WHERE `rooms_has_sensors`.`sensor_id` = " +
                                     info.CurrentSensorID + "";
                        MySqlCommand command = new MySqlCommand(sql, conn);
                        command.ExecuteNonQuery();
                        conn.Close();
                        conn.Open();
                        //current sensor id set avaibility to 0 so it can be assinged to other rooms in the group
                        String sql2 = "UPDATE sensors SET assigned = 0 WHERE sensor_id = " + info.CurrentSensorID +
                                      " ";
                        MySqlCommand command2 = new MySqlCommand(sql2, conn);
                        command2.ExecuteNonQuery();
                        conn.Close();
                        conn.Open();
                        //selected sensor id set avaibility to 1 so it can not be assinged to other rooms in the group
                        String sql3 = "UPDATE sensors SET assigned = 1 WHERE sensor_id = " + info.SensorID + " ";
                        MySqlCommand command3 = new MySqlCommand(sql3, conn);
                        command3.ExecuteNonQuery();
                        conn.Close();
                        conn.Open();
                        //add a record in database that says the selected sensorid is connected to the room
                        String sql4 = "INSERT INTO `rooms_has_sensors` VALUES('" + info.RoomIDSensor + "','" +
                                      info.SensorID + "') ";
                        MySqlCommand command4 = new MySqlCommand(sql4, conn);
                        command4.ExecuteNonQuery();
                        conn.Close();
                    }
                    //selected sensor id is a new one, but the room is currently not assigned to a sensor
                    else if (info.SensorID != "Default" && info.CurrentSensorID == "New")
                    {
                        conn.Open();
                        //current sensor id set avaibility to 1 so it cant be assinged to other rooms in the group
                        String sql = "UPDATE sensors SET assigned = 1 WHERE sensor_id = " + info.SensorID + " ";
                        MySqlCommand command = new MySqlCommand(sql, conn);
                        command.ExecuteNonQuery();
                        conn.Close();
                        conn.Open();
                        //add a record in database that says the selected sensorid is connected to the room
                        String sql2 = "INSERT INTO `rooms_has_sensors` VALUES('" + info.RoomIDSensor + "','" +
                                      info.SensorID + "') ";
                        MySqlCommand command2 = new MySqlCommand(sql2, conn);
                        command2.ExecuteNonQuery();
                        conn.Close();
                    }

                    // Redirect to GroupsAdmin/Profile
                    return RedirectToAction("Profile", "GroupsAdmin", test.GroupID);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("UpdateRoomSensor Exception: |UpdateRoomSensor| {0}", e);
                // Redirects to error page if errors get found
                return RedirectToAction("Error", "Home");
                throw;
            }

        }

        public IActionResult AddRoomInfo(RoomInfo info) // Adds a new room to group
        {

            try
            {
                using (MySqlConnection conn = new MySqlConnection())
                {
                    // The connectionstring with the connection info to login to the server
                    conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                    conn.Open();
                    // Creates query to add all information taken from the filled in textboxes and creates a new entry in the 'rooms' table in database
                    String sql =
                        "INSERT INTO rooms(room_name, room_number, group_id, room_floor, room_facilities,  available, room_comment) VALUES ('"
                        + info.RoomName + "', " + info.RoomNumber + ", 1, " + info.RoomFloor + ", '" + info.RoomFacility + "', 0, '" + info.RoomComment + "'); ";
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                    // Closes the connection after executing the query
                    conn.Close();
                    return RedirectToAction("Index", "GroupsAdmin");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("AddRoomInfo Exception: |AddRoomInfo| {0}", e);
                // Redirects to error page
                return RedirectToAction("Error", "Home");
                throw;
            }


        }

        [ValidateAntiForgeryToken]
        public IActionResult UpdateGroupName(GroupInfo info) // Updates the group name
        {

            try
            {
                using (MySqlConnection conn = new MySqlConnection())
                {
                    // The connectionstring with the connection info to login to the server
                    conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                    conn.Open();
                    // Creates query to Update the Group name into the specified GroupName in the input field
                    String sql =
                        "UPDATE `group` SET group_name = '" + info.GroupName + "' WHERE group_id = '" + info.GroupID + "';";
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                    // Closes the connection after executing the query
                    conn.Close();
                    // Redirects to GroupsAdmin/Profile with the correct GroupID
                    return RedirectToAction("Profile", "GroupsAdmin", info.GroupID);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("UpdateGroupName Exception: |UpdateGroupName| {0}", e);
                // Redirects to the error page
                return RedirectToAction("Error", "Home");
                throw;
            }


        }

        public IActionResult DeactivateUser(string userId, string groupId) // Deactives the specified user
        {

            try
            {
                using (MySqlConnection conn = new MySqlConnection())
                {
                    // The connectionstring with the connection info to login to the server
                    conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                    conn.Open();
                    // Creates query to deactivate a user
                    String sql =
                        "UPDATE user SET user.active = '1' WHERE user.user_id = '" + userId + "';";
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                    // Closes the connection after executing the query
                    conn.Close();
                    return RedirectToAction("Profile", "GroupsAdmin", groupId);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("DeactivateUser Exception", e);
                // Redirects to error page
                return RedirectToAction("Error", "Home");
                throw;
            }


        }

        public IActionResult DeleteReservation(string reservationId, string groupId) // Deletes specified reservation
        {

            try
            {
                using (MySqlConnection conn = new MySqlConnection())
                {
                    // The connectionstring with the connection info to login to the server
                    conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                    conn.Open();
                    // Creates a query to set the specified reservation to invalid
                    String sql =
                        "UPDATE reservations SET reservations.valid = '0'  WHERE reservations.reservation_id = " + reservationId + ";";
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                    // Closes the connection after executing the query
                    conn.Close();
                    // Redirects to GroupsAdmin/Profile
                    return RedirectToAction("Profile", "GroupsAdmin", groupId);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("DeleteReservation Exception", e);
                // Redirects to error page
                return RedirectToAction("Error", "Home");
                throw;
            }  
        }  
    }
}
