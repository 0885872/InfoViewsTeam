using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using Reserveer.Models;

namespace Reserveer.Controllers
{
    public class Database
    {
        public List<string[]> getReservations(string room)
        {
            List<string[]> reservations = new List<string[]>();
            String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            using (MySqlConnection connMysql = new MySqlConnection(connString))
            {
                using (MySqlCommand cmdd = connMysql.CreateCommand())
                {
                    cmdd.CommandText = "SELECT * FROM reservations where room_id = " + room + ";";
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

        public bool VerifyMail(string mail, string token)
        {
            string[] res = new string[1];
            String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
            using (MySqlConnection connMysql = new MySqlConnection(connString))
            {
                using (MySqlCommand cmdd = connMysql.CreateCommand())
                {
                    cmdd.CommandText = "SELECT user_id FROM user WHERE user_mail = '" + mail + "';";
                    cmdd.CommandType = System.Data.CommandType.Text;

                    cmdd.Connection = connMysql;

                    connMysql.Open();

                    using (MySqlDataReader reader = cmdd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            res[0] = reader["user_id"].ToString();
                        }
                    }
                }
                connMysql.Close();
            }
            string[] res2 = new string[2];
            if (res[0] != null)
            {
                using (MySqlConnection connnMysql = new MySqlConnection(connString))
                {
                    using (MySqlCommand cmdd = connnMysql.CreateCommand())
                    {
                        cmdd.CommandText = "SELECT * FROM registration_validation WHERE user_id = " + res[0] + " AND registration_key = '" + token + "';";
                        cmdd.CommandType = System.Data.CommandType.Text;

                        cmdd.Connection = connnMysql;

                        connnMysql.Open();

                        using (MySqlDataReader readerr = cmdd.ExecuteReader())
                        {
                            while (readerr.Read())
                            {

                                res2[0] = readerr["user_id"].ToString();
                                res2[1] = readerr["registration_key"].ToString();
                            }
                        }
                    }
                    connnMysql.Close();
                }
                if (res2[0] != null)
                {
                    string sql = "UPDATE user SET active = 1 WHERE user_id = " + res[0] + ";";
                    using (MySqlConnection conn = new MySqlConnection())
                    {
                        DateTime dateTime = DateTime.Now;
                        string localDate = dateTime.ToString("yyyy/MM/dd HH:mm");
                        conn.ConnectionString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
                        conn.Open();
                        MySqlCommand command = new MySqlCommand(sql, conn);
                        command.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
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
            string sql = "INSERT INTO reservations (room_id,start,end,reservation_date,valid) VALUES (" + reservation.roomid + ",'" + reservation.start + "','" + reservation.end + "','" + localDate + "', 1);";
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

    public List<string[]> getUserGroup()
    {
        List<string[]> UserGroup = new List<string[]>();
        String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        using (MySqlConnection connMysql = new MySqlConnection(connString))
        {
            using (MySqlCommand cmdd = connMysql.CreateCommand())
            {
                int userID = HomeController.UserId;
                cmdd.CommandText = "SELECT group.group_id, group.group_name FROM `group`, `user` where " + userID + " = `user_id` and user.group_id = group.group_id;";
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
                cmdd.CommandText = "SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms` where " + groupID + " = rooms.group_id";
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

    public List<string[]> getGroupUser()
    {
        List<string[]> GroupUser = new List<string[]>();
        String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        using (MySqlConnection connMysql = new MySqlConnection(connString))
        {
            using (MySqlCommand cmdd = connMysql.CreateCommand())
            {
                int groupID = 1;
                cmdd.CommandText = "SELECT user.user_id, user.user_name, user.user_mail, user.active, user.group_id FROM `user` WHERE user.group_id = " + groupID + " AND user.active = 0";
                cmdd.CommandType = System.Data.CommandType.Text;
                //SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms`, `group` where "1" = rooms.group_id ;
                cmdd.Connection = connMysql;

                connMysql.Open();

                using (MySqlDataReader reader = cmdd.ExecuteReader())
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
            connMysql.Close();
            return GroupUser;
        }
    }

    public List<string[]> getGroupAdmin()
    {
        List<string[]> AdminGroup = new List<string[]>();
        String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        using (MySqlConnection connMysql = new MySqlConnection(connString))
        {
            string[] group_id = new string[1];
            using (MySqlCommand cmdd = connMysql.CreateCommand())
            {
                int userID = HomeController.UserId;
                cmdd.CommandText = "SELECT group_id FROM user WHERE user_id = '" + userID + "';";
                cmdd.CommandType = System.Data.CommandType.Text;
                //SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms`, `group` where "1" = rooms.group_id ;
                cmdd.Connection = connMysql;

                connMysql.Open();

                using (MySqlDataReader reader = cmdd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] res = new string[1];
                        group_id[0] = reader["group_id"].ToString();
                    }
                }
                connMysql.Close();
            }

            using (MySqlCommand cmdd = connMysql.CreateCommand())
            {

                cmdd.CommandText = "SELECT group.group_id, group.group_name, domain.domain_name, COUNT(user.user_name) AS user_amount, domain.domain_id FROM `group`, `user`, `domain` where user.group_id = group.group_id and group.group_id = domain.group_id AND group.group_id = '" + group_id[0] + "';";
                cmdd.CommandType = System.Data.CommandType.Text;
                //SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms`, `group` where "1" = rooms.group_id ;
                cmdd.Connection = connMysql;

                connMysql.Open();

                using (MySqlDataReader reader = cmdd.ExecuteReader())
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
                connMysql.Close();
            }

            return AdminGroup;
        }
    }

    public List<string[]> getAdminProfileInfo()
    {
        List<string[]> AdminProfileInfo = new List<string[]>();
        String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        using (MySqlConnection connMysql = new MySqlConnection(connString))
        {
            using (MySqlCommand cmdd = connMysql.CreateCommand())
            {
                int room_id = 1;
                cmdd.CommandText = "SELECT rooms.room_id, rooms.room_name, rooms.room_floor, rooms.available, rooms.room_number, rooms.room_facilities, rooms.room_comment FROM `rooms` WHERE room_id = " + room_id + ";";
                cmdd.CommandType = System.Data.CommandType.Text;
                //SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms`, `group` where "1" = rooms.group_id ;
                cmdd.Connection = connMysql;

                connMysql.Open();

                using (MySqlDataReader reader = cmdd.ExecuteReader())
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
                        AdminProfileInfo.Add(res);
                    }
                }
            }
            connMysql.Close();
            return AdminProfileInfo;
        }
    }



    public List<string[]> getRoomProfileInfo(string room)
    {
        List<string[]> RoomProfileInfo = new List<string[]>();
        String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        using (MySqlConnection connMysql = new MySqlConnection(connString))
        {
            using (MySqlCommand cmdd = connMysql.CreateCommand())
            {
                string room_id = room;
                cmdd.CommandText = "SELECT rooms.room_id, rooms.room_name, rooms.room_floor, rooms.available, rooms.room_number, rooms.room_facilities, rooms.room_comment FROM `rooms` WHERE room_id = " + room_id + ";";
                cmdd.CommandType = System.Data.CommandType.Text;
                //SELECT rooms.room_id, rooms.room_name, rooms.available FROM `rooms`, `group` where "1" = rooms.group_id ;
                cmdd.Connection = connMysql;

                connMysql.Open();

                using (MySqlDataReader reader = cmdd.ExecuteReader())
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
            connMysql.Close();
            return RoomProfileInfo;
        }
    }



    public List<string[]> getRoomReservation()
    {
        List<string[]> RoomReservation = new List<string[]>();
        String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        using (MySqlConnection connMysql = new MySqlConnection(connString))
        {
            using (MySqlCommand cmdd = connMysql.CreateCommand())
            {
                int room_id = 1;
                cmdd.CommandText = "SELECT user.user_name, reservations.reservation_id, reservations.start, reservations.end, reservations.reservation_date, reservations.valid FROM `user_has_reservations`, user, reservations WHERE user.user_id = user_has_reservations.user_id and user_has_reservations.reservation_id = reservations.reservation_id AND reservations.room_id = '" + room_id + "';";
                cmdd.CommandType = System.Data.CommandType.Text;
                cmdd.Connection = connMysql;

                connMysql.Open();

                using (MySqlDataReader reader = cmdd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] res = new string[6];
                        res[0] = reader["user_name"].ToString();
                        res[1] = reader["start"].ToString();
                        res[2] = reader["end"].ToString();
                        res[3] = reader["reservation_date"].ToString();
                        res[4] = reader["valid"].ToString();
                        res[5] = reader["reservation_id"].ToString();
                        RoomReservation.Add(res);
                    }
                }
            }
            connMysql.Close();
            return RoomReservation;
        }
    }

    public List<string[]> getGroupRoomReservation()
    {
        List<string[]> GroupRoomReservation = new List<string[]>();
        String connString = "Server=drakonit.nl;Database=timbrrf252_roomreserve;Uid=timbrrf252_ictlab;Password=ictlabhro;SslMode=none";
        using (MySqlConnection connMysql = new MySqlConnection(connString))
        {
            using (MySqlCommand cmdd = connMysql.CreateCommand())
            {
                int group_id = 1;
                cmdd.CommandText = "SELECT reservations.reservation_id, user.user_name,  reservations.start, reservations.end, reservations.reservation_date, reservations.valid, rooms.room_id, rooms.room_name, rooms.group_id FROM `rooms`, `user_has_reservations`, `user`, `reservations` WHERE user.user_id = user_has_reservations.user_id and user_has_reservations.reservation_id = reservations.reservation_id AND reservations.room_id =" + group_id + " AND reservations.valid = 0;";
                cmdd.CommandType = System.Data.CommandType.Text;
                cmdd.Connection = connMysql;

                connMysql.Open();

                using (MySqlDataReader reader = cmdd.ExecuteReader())
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
            connMysql.Close();
            return GroupRoomReservation;
        }
    }

}
}
    