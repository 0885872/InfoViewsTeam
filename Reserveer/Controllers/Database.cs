using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Reserveer.Models;

namespace Reserveer.Controllers
{
    public class Database
    {
        public List<string[]> getReservations()
        {
            List<string[]> reservations = new List<string[]>();
            String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            using (MySqlConnection connMysql = new MySqlConnection(connString))
            {
                using (MySqlCommand cmdd = connMysql.CreateCommand())
                {
                    cmdd.CommandText = "SELECT * FROM reservations";
                    cmdd.CommandType = System.Data.CommandType.Text;

                    cmdd.Connection = connMysql;

                    connMysql.Open();

                    using (MySqlDataReader reader = cmdd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] res = new string[5];
                            res[0] = reader["reservation_id"].ToString();
                            res[1] = reader["room_id"].ToString();
                            res[2] = reader["start"].ToString();
                            res[3] = reader["end"].ToString();
                            res[4] = reader["valid"].ToString();
                            res[2] = Convert.ToDateTime(res[2]).ToString("yyyy/MM/dd HH:mm");
                            res[3] = Convert.ToDateTime(res[3]).ToString("yyyy/MM/dd HH:mm");
                            reservations.Add(res);
                        }
                    }
                }
                connMysql.Close();
                return reservations;
            }
        }

        public void setReservations(ReservationModel reservation)
        {
            using (MySqlConnection conn = new MySqlConnection())
            {
                DateTime dateTime = DateTime.Now;
                string localDate = dateTime.ToString("yyyy/MM/dd HH:mm");
                conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                conn.Open();
                string sql = "INSERT INTO reservations (room_id,start,end,reservation_date,valid) VALUES (1,'" + reservation.start + "','" + reservation.end + "','" + localDate + "', 1);";
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
        }

        public string[] FindDuplicates(UserRegistration user)
        {
            string[] result = new string[1];
            String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            using (MySqlConnection connMysql = new MySqlConnection(connString))
            {
                using (MySqlCommand cmdd = connMysql.CreateCommand())
                {

                    cmdd.CommandText = "SELECT COUNT(user_mail) AS res FROM user WHERE user_mail = '" + user.Mail + "';";
                    cmdd.CommandType = System.Data.CommandType.Text;

                    cmdd.Connection = connMysql;

                    connMysql.Open();

                    using (MySqlDataReader reader = cmdd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            result[0] = reader["res"].ToString();

                        }
                    }
                }
            }
            return result;
        }

        public string getDomainCheck(string domain)
        {
            string[] res = new string[1];
            string result;
            String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            using (MySqlConnection connMysql = new MySqlConnection(connString))
            {
                using (MySqlCommand cmdd = connMysql.CreateCommand())
                {

                    cmdd.CommandText = "SELECT group_id FROM domain WHERE domain_name = '" + domain + "';";
                    cmdd.CommandType = System.Data.CommandType.Text;

                    cmdd.Connection = connMysql;

                    connMysql.Open();

                    using (MySqlDataReader reader = cmdd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            res[0] = reader["group_id"].ToString();
                        }
                    }
                    connMysql.Close();
                }
                    
                    if (res[0] == null)
                    {
                        result = "null";
                    }
                    else
                    {
                        result = res[0];
                    }
                    return result;
                }
            }
        }


        public List<string[]> getUserGroup()
        {
            List<string[]> UserGroup = new List<string[]>();
            String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            using (MySqlConnection connMysql = new MySqlConnection(connString))
            {
                using (MySqlCommand cmdd = connMysql.CreateCommand())
                {
                    int userID = 27;
                    cmdd.CommandText = "SELECT group.group_id, group.group_name FROM `group`, `user` where "+ userID + " = `user_id` and user.group_id = group.group_id;";
                    cmdd.CommandType = System.Data.CommandType.Text;
                    //SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms`, `group` where "1" = rooms.group_id ;
                    cmdd.Connection = connMysql;

                    connMysql.Open();

                    using (MySqlDataReader reader = cmdd.ExecuteReader())
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
                return UserGroup;
            }
        }

        public List<string[]> getGroupRooms()
        {
            List<string[]> GroupRooms = new List<string[]>();
            String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            using (MySqlConnection connMysql = new MySqlConnection(connString))
            {
                using (MySqlCommand cmdd = connMysql.CreateCommand())
                {
                    int groupID = 1;
                    cmdd.CommandText = "SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms`, `group` where "+ groupID+" = rooms.group_id";
                    cmdd.CommandType = System.Data.CommandType.Text;
                    //SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms`, `group` where "1" = rooms.group_id ;
                    cmdd.Connection = connMysql;

                    connMysql.Open();

                    using (MySqlDataReader reader = cmdd.ExecuteReader())
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
                connMysql.Close();
                return GroupRooms;
            }
        }


    }
    