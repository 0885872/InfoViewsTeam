using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserveer.Models;
using Newtonsoft.Json;
using System.Net.Mail;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Reserveer.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class ScheduleController : Controller
    {
        //Get reservations
        [HttpGet]
        public ActionResult Index()
        {

            try
            {
                int userid = HomeController.UserId;
                string room = Request.Query["roomid"];

                //QRCode Code nog commenten
                var request = HttpContext.Request;
                using (MemoryStream ms = new MemoryStream())
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();

                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://145.24.222.130" + request.Path + "?RoomId=" + room, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);


                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        qrCodeImage.Save(ms, ImageFormat.Png);
                        ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }

                Database db = new Database();
                List<string[]> results = db.getReservations(room);
                var json = JsonConvert.SerializeObject(results);
                ViewData["results"] = json; 

                return View();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Index Schedule Exception: {0}", e);
                return RedirectToAction("Error", "Home");
                throw;
            }




        }

        //Reservations post
        [HttpPost]
        public ActionResult SetReservation([FromBody]ReservationModel reservation)
        {
            {
                try
                {
                    int useridd = HomeController.UserId;
                    string userid = useridd.ToString();
                    ReservationModel reservations = new ReservationModel
                    {
                        title = reservation.title,
                        start = reservation.start,
                        end = reservation.end,
                        roomid = reservation.roomid,
                        userid = userid


                    };
                    Database db = new Database();
                    db.setReservations(reservations);
                    string mailaddr = db.getUserMail(userid);
                    string roomname = db.getRoomName(reservation.roomid);
                    MailMessage msg = new MailMessage();
                    SmtpClient smtp = new SmtpClient();

                    msg.From = new MailAddress("Noreply@infoviews.drakonit.nl");
                    msg.To.Add(mailaddr);
                    msg.Subject = "Confirmation of reservation";
                    msg.Body = "Hi there, We would like to inform you: We've saved your reservation! start: " + reservation.start + ", end: " + reservation.end + ", room: " + roomname + ". Thanks for using InfoViews!";

                    var client = new SmtpClient("smtp.hro.nl", 25);
                    client.Send(msg);

                    return View("Index");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("SetReservation Exception: {0}", e);
                    return RedirectToAction("Error", "Home");
                    throw;
                }
            }

        }

    }
}