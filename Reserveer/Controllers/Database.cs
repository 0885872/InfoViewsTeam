using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite.Internal.UrlActions;
using MySql.Data.MySqlClient;
using Reserveer.Models;

namespace Reserveer.Controllers
{
    //This is the class used for database connections. It contains methods to get, update and set values in the database.
    public class Database
    {
        //String with connection values (server login)
        String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";

        public string[] getLatestTemperature(string roomid) // Gets the latest temperature in the room from the Raspberry Pi
        {

            try
            {
                string[] res = new string[2]; //String array to store results
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup the connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        // Selects the latest sensor temperature value and Time that has been updated to the room sensor
                        cmdd.CommandText = "SELECT sv.value, sv.datetime as date FROM sensor_values sv, sensors s, rooms_has_sensors r WHERE sv.sensor_id = s.sensor_id AND s.sensor_id = r.sensor_id AND r.room_id = " + roomid + " ORDER BY date DESC LIMIT 1;";
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Opening the connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execute the query and store the results
                        {
                            while (reader.Read())
                            {

                                res[0] = reader["value"].ToString(); // Places the temperature value into the string array
                                res[1] = reader["date"].ToString(); // Places the date into the string array
                            }
                        }
                    }
                    connMysql.Close(); //Closing the connection
                    return res; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getLatestTemperature Exception: {0}", e);

                throw;
            }


        }

        public string getUserMail(string id) // Get the mail corresponding to the userID
        {

            try
            {
                string[] res = new string[1]; //string array to store the results
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup the connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        // Select the user_mail that is connected to the user_id
                        cmdd.CommandText = "SELECT user_mail FROM user where user_id = " + id + ";";
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Opening the connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execution of the query and storing the result
                        {
                            while (reader.Read())
                            {

                                res[0] = reader["user_mail"].ToString(); // Puts the selected user_mail into the string array
                            }
                        }
                    }
                    connMysql.Close(); //Closing the connection
                    return res[0]; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getUserMail Exception: {0}", e);

                throw;
            }


        }

        public string getRoomName(string id) // Gets the room name corresponding to the roomID
        {

            try
            {
                string[] res = new string[1]; //String array to store the results
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup the connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        cmdd.CommandText = "SELECT room_name FROM rooms where room_id = " + id + ";"; // Query to get the room name from rooms-table with corresponding room_id
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Opening the connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Executihon of the query and storing the result
                        {
                            while (reader.Read())
                            {

                                res[0] = reader["room_name"].ToString();
                            }
                        }
                    }
                    connMysql.Close(); //Close the connection
                    return res[0]; //Return the results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getRoomName Exception: {0}", e);
                throw;
            }


        }

        public List<string[]> getReservations(string room) // Gets all reservations corresponding to the roomID
        {

            try
            {
                List<string[]> reservations = new List<string[]>(); //List of string arrays to store the results
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup the connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        //Query to get reservation data from database, associated with the given room id.
                        cmdd.CommandText = "SELECT reservations.reservation_id, reservations.room_id, reservations.start, reservations.end, reservations.valid, rooms.room_name FROM reservations, rooms where reservations.room_id = rooms.room_id AND reservations.room_id = " + room + " ORDER BY reservations.start DESC;";
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open the connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execution of the sql query and storage of the results of the sql query execution
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[6];
                                res[0] = reader["reservation_id"].ToString();
                                res[1] = reader["room_id"].ToString();
                                res[2] = reader["start"].ToString();
                                res[3] = reader["end"].ToString();
                                res[4] = reader["valid"].ToString();
                                res[5] = reader["room_name"].ToString();
                                res[2] = Convert.ToDateTime(res[2]).ToString("yyyy/MM/dd HH:mm");
                                res[3] = Convert.ToDateTime(res[3]).ToString("yyyy/MM/dd HH:mm");
                                reservations.Add(res);
                            }
                        }
                    }
                    connMysql.Close(); //Close the connection
                    return reservations; //Return the List of string []'s containing results from database
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getReservations Exception: {0}", e);
                throw;
            }


        }

        public List<string[]> getUserReservations() // Gets all reservations corresponding to the roomID
        {

            try
            {
                List<string[]> reservations = new List<string[]>(); //List of string []'s to store results of database query
                using (MySqlConnection connMysql = new MySqlConnection(connString))//Setup database connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        int userID = HomeController.UserId; //Get user id of logged in user
                        cmdd.CommandText = "SELECT reservations.reservation_id, reservations.room_id, reservations.start, reservations.end, reservations.reservation_date, user.group_id, rooms.room_name FROM rooms, reservations, user, user_has_reservations where rooms.room_id = reservations.room_id AND user.user_id = user_has_reservations.user_id AND reservations.reservation_id = user_has_reservations.reservation_id AND reservations.valid = 1 AND user.user_id = " + userID + " ORDER BY reservations.reservation_date DESC"; //Sql query to be executed : all reservations of a room.
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execution of given sql query and storage of results in List of string []'s
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[7];
                                res[0] = reader["reservation_id"].ToString();
                                res[1] = reader["room_id"].ToString();
                                res[2] = reader["start"].ToString();
                                res[3] = reader["end"].ToString();
                                res[4] = reader["reservation_date"].ToString();
                                res[5] = reader["group_id"].ToString();
                                res[6] = reader["room_name"].ToString();
                                reservations.Add(res);
                            }
                        }
                    }
                    connMysql.Close(); //Close database connection
                    return reservations; //Return List of string []'s with database results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getUserReservations Exception", e);
                throw;
            }


        }

        public bool VerifyMail(string mail, string token) // Verifies user emailaccount by combination with a token
        {

            try
            {
                string[] res = new string[1]; //String array to store database results
                using (MySqlConnection connMysql = new MySqlConnection(connString)) // setup the connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        cmdd.CommandText = "SELECT user_id FROM user WHERE user_mail = '" + mail + "';"; //Sql query to be executed ; get user id from user_mail
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execution of the reader and storage of database results
                        {
                            while (reader.Read())
                            {

                                res[0] = reader["user_id"].ToString();
                            }
                        }
                    }
                    connMysql.Close(); //Close the connection
                }
                string[] res2 = new string[2];//String array to store database results
                if (res[0] != null) //Condition: Only execute code below if result of previous query was received and stored.
                {
                    using (MySqlConnection connnMysql = new MySqlConnection(connString))//Setup connection
                    {
                        using (MySqlCommand cmdd = connnMysql.CreateCommand())
                        {
                            cmdd.CommandText = "SELECT * FROM registration_validation WHERE user_id = " + res[0] + " AND registration_key = '" + token + "';"; //Sql query to execute; Check if validation record exists
                            cmdd.CommandType = System.Data.CommandType.Text;

                            cmdd.Connection = connnMysql;

                            connnMysql.Open(); // Open connection

                            using (MySqlDataReader readerr = cmdd.ExecuteReader()) //Execution of the reader and storage of database results
                            {
                                while (readerr.Read())
                                {

                                    res2[0] = readerr["user_id"].ToString();
                                    res2[1] = readerr["registration_key"].ToString();
                                }
                            }
                        }
                        connnMysql.Close(); //Close the connection
                    }
                    if (res2[0] != null)//Condition: Only execute code below if result of previous query was received and stored.
                    {
                        string sql = "UPDATE user SET active = 1 WHERE user_id = " + res[0] + ";"; //Sql query to execute; Set user account to active 
                        using (MySqlConnection conn = new MySqlConnection()) //Setup connection
                        {
                            DateTime dateTime = DateTime.Now;
                            string localDate = dateTime.ToString("yyyy/MM/dd HH:mm");
                            conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                            conn.Open();
                            MySqlCommand command = new MySqlCommand(sql, conn);
                            command.ExecuteNonQuery(); //Execution of the sql query
                            conn.Close(); //Close the connection
                        }

                        string validationSql = "DELETE FROM registration_validation WHERE user_id = " + res2[0] + ";";//Sql query to be executed ; Delete possibility to verify account after activation
                        using (MySqlConnection conn3 = new MySqlConnection()) //Setup the connection
                        {
                            conn3.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                            conn3.Open();
                            MySqlCommand command3 = new MySqlCommand(validationSql, conn3);
                            command3.ExecuteNonQuery(); //Execution of the sql query
                            conn3.Close(); //Close the connection
                        }
                        return true; //Return true -> All executed

                    }
                    else
                    {
                        return false; //Return false -> condition not satisfied
                    }
                }
                else
                {
                    return false; //Return false -> condition not satisfied
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("VerifyMail Exception: {0}", e);
                throw;
            }


        }

        public void setReservations(ReservationModel reservation) // Sets the reservations made by users
        {
            //Save reservation to database

            try
            {
                using (MySqlConnection conn = new MySqlConnection()) //Setup the connection
                {
                    DateTime dateTime = DateTime.Now; //Introducing a datetime object of current date/time
                    string localDate = dateTime.ToString("yyyy/MM/dd HH:mm"); //String of current date/time object
                    conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                    conn.Open(); //Open the connection
                    string sql = "INSERT INTO reservations (room_id,start,end,reservation_date,valid) VALUES (" + reservation.roomid + ",'" + reservation.start + "','" + reservation.end + "','" + localDate + "', 1);"; //Sql query to be executed ; data of booking
                    MySqlCommand command = new MySqlCommand(sql, conn);
                    command.ExecuteNonQuery(); //Executing of sql query
                    conn.Close();
                }



                //Get reservation id for next step
                string[] result = new string[1]; //string array to store database result
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup the connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {

                        cmdd.CommandText = "SELECT reservation_id FROM reservations WHERE room_id = " + reservation.roomid + " AND start = '" + reservation.start + "' AND end = '" + reservation.end + "';"; //Sql query to exectute ; Getting reservation id
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open();// Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execution of sql query and storage of database results
                        {
                            while (reader.Read())
                            {

                                result[0] = reader["reservation_id"].ToString();

                            }
                        }
                    }
                }

                //user-reservation assignment
                using (MySqlConnection connn = new MySqlConnection()) //Setup the connection
                {
                    connn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                    connn.Open(); //Open the connection
                    string sql = "INSERT INTO user_has_reservations (user_id,reservation_id) VALUES (" + reservation.userid + "," + result[0] + ");"; //Sql query to execute
                    MySqlCommand command = new MySqlCommand(sql, connn);
                    command.ExecuteNonQuery();//Execution of sql query
                    connn.Close(); //Close connectin
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("setReservations Exception: {0}", e);
                throw;
            }


        }

        public string[] FindDuplicates(UserRegistration user) // Checks for duplicate user email
        {

            try
            {
                string[] result = new string[1]; //String array to store database results
                using (MySqlConnection connMysql = new MySqlConnection(connString))
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand()) //Setup the connection
                    {

                        cmdd.CommandText = "SELECT COUNT(user_mail) AS res FROM user WHERE user_mail = '" + user.Mail + "';"; //Sql query to execute 
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open();//Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Sql query execution and data storage
                        {
                            while (reader.Read())
                            {

                                result[0] = reader["res"].ToString();

                            }
                        }
                    }
                }
                return result; //Return string [] with database result
            }
            catch (Exception e)//Exception catcher
            {
                Debug.WriteLine("FindDuplicates Exception: {0}", e);
                throw;
            }


        }

        public string getDomainCheck(string domain) // Checks for domain name corresponding with group_id
        {

            try
            {
                string[] res = new string[1];//String array to store database results
                string result;
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup the connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {

                        cmdd.CommandText = "SELECT group_id FROM domain WHERE domain_name = '" + domain + "';";//Sql query to execute; Find group_id with domain name
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connMysql;

                        connMysql.Open();// Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execute sql query and store results
                        {
                            while (reader.Read())
                            {

                                res[0] = reader["group_id"].ToString();
                            }
                        }
                        connMysql.Close(); //close connection
                    }

                    if (res[0] == null)//Condition: If no result
                    {
                        result = "null";
                    }
                    else//Condition: If result
                    {
                        result = res[0];
                    }
                    return result; //Return result variable
                }
            }
            catch (Exception e)//Exception catcher
            {
                Debug.WriteLine("getDomainCheck Exception: {0}", e);
                throw;
            }


        }

        public List<string[]> getUserGroup() // Gets usergroup corresponding with userID
        {

            try
            {
                List<string[]> UserGroup = new List<string[]>(); //List of string arrays to store results
                using (MySqlConnection connMysql = new MySqlConnection(connString))//Setup connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        int userID = HomeController.UserId;
                        cmdd.CommandText = "SELECT group.group_id, group.group_name FROM `group`, `user` where " +
                                           userID + " = `user_id` and user.group_id = group.group_id;"; //Sql query to execute ; Get usergroup corresponding with userID
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open();//Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execute sql query and store results
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[2];
                                res[0] = reader["group_id"].ToString();
                                res[1] = reader["group_name"].ToString();
                                UserGroup.Add(res);
                            }
                        }
                    }

                    connMysql.Close();
                    return UserGroup; //Return database results
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("getUserGroup Exception: {0}", e);
                throw;
            }


        }
        public List<string[]> getUserInfo() // Gets usergroup corresponding with userID
        {

            try
            {
                List<string[]> UserInfo = new List<string[]>(); //List of string arrays to store results
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        int userID = HomeController.UserId;
                        cmdd.CommandText = "SELECT user_name, user_mail FROM `user` WHERE user_id = " + userID + " "; //Sql query to execute ; get user info corresponding with userId
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open();

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execution of sql query and result data storage
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[2];
                                res[0] = reader["user_name"].ToString();
                                res[1] = reader["user_mail"].ToString();
                                UserInfo.Add(res);
                            }
                        }
                    }
                    connMysql.Close();
                    return UserInfo; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getUserInfo Exception: {0}", e);
                throw;
            }


        }


        public List<string[]> getCurrentRoomSensors(string room) // Gets the assigned temperature sensors/raspberry corresponding to the roomid
        {

            try
            {
                List<string[]> AvaibleRoomSensors = new List<string[]>(); //List of string array's to store results
                using (MySqlConnection connMysql = new MySqlConnection(connString))
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())//Setup connection
                    {
                        //Below: sql query to execute ; Get assigned sensor information
                        cmdd.CommandText = "SELECT sensors.sensor_id, sensors.mac, sensors.group_id, sensors.assigned FROM `sensors`,`rooms_has_sensors` WHERE sensors.sensor_id = rooms_has_sensors.sensor_id AND sensors.assigned = 1 AND rooms_has_sensors.room_id = " + room + "";
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open();

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Sql query execution and storage of result data
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[4];
                                res[0] = reader["sensor_id"].ToString();
                                res[1] = reader["mac"].ToString();
                                res[2] = reader["group_id"].ToString();
                                res[3] = reader["assigned"].ToString();
                                AvaibleRoomSensors.Add(res);
                            }
                        }
                    }
                    connMysql.Close();// Close connection
                    return AvaibleRoomSensors; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getCurrentRoomSensors Exception: {0}", e);
                throw;
            }


        }

        public List<string[]> getAvaibleRoomSensors(string room) // Gets all temperature sensors/raspberry that are unassigned
        {

            try
            {
                List<string[]> AvaibleRoomSensors = new List<string[]>(); //List of string []'s to store results from database
                using (MySqlConnection connMysql = new MySqlConnection(connString))
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        //moet nog uit geselecteerd worden uit specifieke groep

                        cmdd.CommandText = "SELECT sensors.sensor_id, sensors.mac, sensors.group_id, sensors.assigned FROM `sensors` WHERE sensors.assigned = 0 "; //Sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Sql query execution and result storage
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[4];
                                res[0] = reader["sensor_id"].ToString();
                                res[1] = reader["mac"].ToString();
                                res[2] = reader["group_id"].ToString();
                                res[3] = reader["assigned"].ToString();
                                AvaibleRoomSensors.Add(res);
                            }
                        }
                    }
                    connMysql.Close(); //Close connection
                    return AvaibleRoomSensors; //Return result
                }
            }
            catch (Exception e) // Exception catcher
            {
                Debug.WriteLine("getAvaibleRoomSensors Exception: {0}", e);
                throw;
            }


        }

        public List<string[]> getGroupRooms() // Gets all rooms corresponding to the groupID
        {
            try
            {
                List<string[]> GroupRooms = new List<string[]>();
                using (MySqlConnection connMysql = new MySqlConnection(connString))
                {
                    string[] group_id = new string[1]; //string array to store database results
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        int userID = HomeController.UserId;
                        cmdd.CommandText = "SELECT group_id FROM user WHERE user_id = '" + userID + "';"; //sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Sql query execution and result data storage
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[1];
                                group_id[0] = reader["group_id"].ToString();
                            }
                        }
                        connMysql.Close(); //Close connection
                    }

                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        cmdd.CommandText = "SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms` where " + group_id[0] + " = rooms.group_id"; //Sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //sql query execution and data storage
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[3];
                                res[0] = reader["room_id"].ToString();
                                res[1] = reader["room_name"].ToString();
                                res[2] = reader["available"].ToString();
                                GroupRooms.Add(res);
                            }
                        }
                    }
                    connMysql.Close(); //Close connection
                    return GroupRooms; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getGroupRooms Exception: {0}", e);
                throw;
            }
        }



        public List<string[]> getGroupUser() // Gets all users corresponding to the groupID
        {
            try
            {
                List<string[]> GroupUser = new List<string[]>();
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup connection
                {
                    string[] group_id = new string[1]; //String array to store datbase result
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        int userID = HomeController.UserId;
                        cmdd.CommandText = "SELECT group_id FROM user WHERE user_id = '" + userID + "';"; //Sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Sql query execution en result data storage
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[1];
                                group_id[0] = reader["group_id"].ToString();
                            }
                        }
                        connMysql.Close(); //Close connection
                    }

                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        cmdd.CommandText = "SELECT user.user_id, user.user_name, user.user_mail, user.active, user.group_id FROM `user` WHERE user.group_id = " + group_id[0] + " AND user.active = 0"; //Sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execute the sql query and store results
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[5];
                                res[0] = reader["user_id"].ToString();
                                res[1] = reader["user_name"].ToString();
                                res[2] = reader["user_mail"].ToString();
                                res[3] = reader["active"].ToString();
                                res[4] = reader["group_id"].ToString();
                                GroupUser.Add(res);
                            }
                        }
                    }
                    connMysql.Close(); //Close connection
                    return GroupUser; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getGroupUser Exception: {0}", e);
                throw;
            }
        }



        public List<string[]> getGroupAdmin() // Get the groups corresponding to the Admin groupID
        {

            try
            {
                List<string[]> AdminGroup = new List<string[]>(); //List of string []'s to store results
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //Setup connection
                {
                    string[] group_id = new string[1]; //String [] to store db result of group id
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        int userID = HomeController.UserId;
                        cmdd.CommandText = "SELECT group_id FROM user WHERE user_id = '" + userID + "';"; //Sql query to be executed ; get group_d corresponding to user_id
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execute the sql query and store result in string []
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[1];
                                group_id[0] = reader["group_id"].ToString();
                            }
                        }
                        connMysql.Close(); //Close connection
                    }

                    using (MySqlCommand cmdd = connMysql.CreateCommand()) //Setup connection
                    {

                        cmdd.CommandText = "SELECT group.group_id, group.group_name, domain.domain_name, COUNT(user.user_name) AS user_amount, domain.domain_id FROM `group`, `user`, `domain` where user.group_id = group.group_id and group.group_id = domain.group_id AND group.group_id = '" + group_id[0] + "';"; //Sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Open connection

                        using (MySqlDataReader reader = cmdd.ExecuteReader())//Execute sql query and store results
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[5];
                                res[0] = reader["group_id"].ToString();
                                res[1] = reader["group_name"].ToString();
                                res[2] = reader["domain_name"].ToString();
                                res[3] = reader["user_amount"].ToString();
                                res[4] = reader["domain_id"].ToString();
                                AdminGroup.Add(res);
                            }
                        }
                        connMysql.Close(); //close connection
                    }

                    return AdminGroup; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getGroupAdmin Exception: {0}", e);
                throw;
            }



        }

        public List<string[]> getRoomProfileInfo(string room) // Gets the room information corresponding to te roomID
        {

            try
            {
                List<string[]> RoomProfileInfo = new List<string[]>(); //List of string []'s to store result data
                using (MySqlConnection connMysql = new MySqlConnection(connString))//Setip connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        string room_id = room; //Get roomid from parameter
                        cmdd.CommandText = "SELECT rooms.room_id, rooms.room_name, rooms.room_floor, rooms.available, rooms.room_number, rooms.room_facilities, rooms.room_comment FROM `rooms` WHERE room_id = " + room_id + ";"; //Sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Connection open

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Execute sql query and store the rsults
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[7];
                                res[0] = reader["room_name"].ToString();
                                res[1] = reader["room_floor"].ToString();
                                res[2] = reader["available"].ToString();
                                res[3] = reader["room_number"].ToString();
                                res[4] = reader["room_facilities"].ToString();
                                res[5] = reader["room_comment"].ToString();
                                res[6] = reader["room_id"].ToString();
                                RoomProfileInfo.Add(res);
                            }
                        }
                    }
                    connMysql.Close(); //Close connection
                    return RoomProfileInfo; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getRoomProfileInfo Exception: {0}", e);
                throw;
            }


        }



        public List<string[]> getRoomReservation() // Gets all info corresponding to reservationID's
        {

            try
            {
                List<string[]> RoomReservation = new List<string[]>(); //List of string [] 's to store results
                using (MySqlConnection connMysql = new MySqlConnection(connString)) //setup connection
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand()) //Setup 
                    {
                        int room_id = 1;
                        cmdd.CommandText = "SELECT user.user_name, reservations.reservation_id, reservations.start, reservations.end, reservations.reservation_date, reservations.valid, rooms.group_id FROM `user_has_reservations`, user, reservations, rooms WHERE reservations.valid = 1 AND user.user_id = user_has_reservations.user_id and user_has_reservations.reservation_id = reservations.reservation_id AND rooms.room_id = reservations.room_id AND reservations.room_id = '" + room_id + "';"; //Sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open();

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //Sql query execution and result data storage
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[7];
                                res[0] = reader["user_name"].ToString();
                                res[1] = reader["start"].ToString();
                                res[2] = reader["end"].ToString();
                                res[3] = reader["reservation_date"].ToString();
                                res[4] = reader["valid"].ToString();
                                res[5] = reader["reservation_id"].ToString();
                                res[6] = reader["group_id"].ToString();
                                RoomReservation.Add(res);
                            }
                        }
                    }
                    connMysql.Close(); //Close connection
                    return RoomReservation; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getRoomReservation Exception: {0}", e);
                throw;
            }


        }

        public List<string[]> getGroupRoomReservation() // Gets all reservation info corresponding to reservationID
        {

            try
            {
                List<string[]> GroupRoomReservation = new List<string[]>(); //List of string []'s to store database results
                using (MySqlConnection connMysql = new MySqlConnection(connString))//Connection setup
                {
                    using (MySqlCommand cmdd = connMysql.CreateCommand())
                    {
                        int group_id = 1;
                        cmdd.CommandText = "SELECT reservations.reservation_id, user.user_name,  reservations.start, reservations.end, reservations.reservation_date, reservations.valid, rooms.room_id, rooms.room_name, rooms.group_id FROM `rooms`, `user_has_reservations`, `user`, `reservations` WHERE user.user_id = user_has_reservations.user_id and user_has_reservations.reservation_id = reservations.reservation_id AND reservations.room_id =" + group_id + " AND reservations.valid = 1;"; //Sql query to execute
                        cmdd.CommandType = System.Data.CommandType.Text;
                        cmdd.Connection = connMysql;

                        connMysql.Open(); //Connection open

                        using (MySqlDataReader reader = cmdd.ExecuteReader()) //sql query execution and result data storage
                        {
                            while (reader.Read())
                            {
                                string[] res = new string[9];
                                res[0] = reader["reservation_id"].ToString();
                                res[1] = reader["user_name"].ToString();
                                res[2] = reader["start"].ToString();
                                res[3] = reader["end"].ToString();
                                res[4] = reader["reservation_date"].ToString();
                                res[5] = reader["valid"].ToString();
                                res[6] = reader["room_id"].ToString();
                                res[7] = reader["room_name"].ToString();
                                res[8] = reader["group_id"].ToString();
                                GroupRoomReservation.Add(res);
                            }
                        }
                    }
                    connMysql.Close();//Close connection
                    return GroupRoomReservation; //Return results
                }
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("getGroupRoomReservation Exception: {0}", e);
                throw;
            }


        }

    }
}
