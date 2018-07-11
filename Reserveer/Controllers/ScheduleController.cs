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
        //Called when Schedule route is requested
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                int userid = HomeController.UserId; //Get userid of logged in user
                string room = Request.Query["roomid"]; //Get roomid from url parameter

                var request = HttpContext.Request;
                using (MemoryStream ms = new MemoryStream()) //QR-code generation
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

                Database db = new Database(); // Make instance of database class
                List<string[]> results = db.getReservations(room); // Get reservation from database object and store returned values in List of string []'s
                var json = JsonConvert.SerializeObject(results); //Convert previous list of string []'s to json object
                ViewData["results"] = json; //Hands over json object to view

                return View(); //View gets returned with viewdata
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("Index Schedule Exception: {0}", e);
                return RedirectToAction("Error", "Home");
                throw;
            }




        }

        //Reservations post
        //Method to save reservations to database
        [HttpPost]
        public ActionResult SetReservation([FromBody]ReservationModel reservation)
        {

            try
            {
                int useridd = HomeController.UserId; //Get userid of logged in user
                string userid = useridd.ToString();
                ReservationModel reservations = new ReservationModel //Assign data to model
                {
                    title = reservation.title,
                    start = reservation.start,
                    end = reservation.end,
                    roomid = reservation.roomid,
                    userid = userid


                };
                Database db = new Database(); //Create new instance of database object
                db.setReservations(reservations); //Save data in database
                string mailaddr = db.getUserMail(userid); //Get and store user mailadres in string var
                string roomname = db.getRoomName(reservation.roomid); //Get and store roomname in string var
                MailMessage msg = new MailMessage(); //Create email message
                SmtpClient smtp = new SmtpClient(); //Create SMTP client

                //Setup of email
                msg.From = new MailAddress("Noreply@infoviews.drakonit.nl");
                msg.To.Add(mailaddr);
                msg.Subject = "Confirmation of reservation";
                msg.Body = "Hi there, We would like to inform you: We've saved your reservation! start: " + reservation.start + ", end: " + reservation.end + ", room: " + roomname + ". Thanks for using InfoViews!";

                var client = new SmtpClient("smtp.hro.nl", 25); //Configure SMTP mailserver
                client.Send(msg); //Send confirmation mail 
                return RedirectToAction("Index"); //Returns the view Index from Schedule Controller 
            }
            catch (Exception e) //Exception catcher
            {
                Debug.WriteLine("SetReservation Exception: {0}", e);
                return RedirectToAction("Error", "Home"); //Shows error page
                throw;
            }


        }

    }
}