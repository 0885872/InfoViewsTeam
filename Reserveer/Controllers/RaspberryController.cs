using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;

namespace Reserveer.Controllers
{
    public class RaspberryController : Controller
    {
        //Reservaties ophalen
        [HttpGet]
        public ActionResult Index()
        {
            //Called when Raspberry route requested
            {
                try
                {
                    string room = Request.Query["roomid"]; //Get roomid from url parameter

                    //QRCode Code nog commenten
                    var request = HttpContext.Request;
                    using (MemoryStream ms = new MemoryStream()) //setup qr code generator
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();

                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://145.24.222.130/Schedule?RoomId=" + room, QRCodeGenerator.ECCLevel.Q); //Data for qr code used to generate qr code.
                        QRCode qrCode = new QRCode(qrCodeData); 


                        using (Bitmap qrCodeImage = qrCode.GetGraphic(20))//Generate QR code
                        {
                            qrCodeImage.Save(ms, ImageFormat.Png);
                            ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                        }
                    }

                    Database db = new Database(); //Make new instance of database class
                    List<string[]> results = db.getReservations(room); //Calls getReservations method of database class and stores returned values as list of string []'s
                    var json = JsonConvert.SerializeObject(results); //Converts prevoius list of string []'s to json object
                    ViewData["results"] = json; //Hands over json object to view

                    string[] temperatureData = db.getLatestTemperature(room); //Calls getLatestTemperature method of database class and stores returned values in string []
                    var jsonTemp = JsonConvert.SerializeObject(temperatureData); //Converts prevoius string [] to json object
                    ViewData["temp"] = jsonTemp; //Hands over json object to view
                    return View(); //Returns view

                }
                catch (Exception e) //Exception catcher
                {
                    Debug.WriteLine("Index raspberry Exception: {0}", e);
                    return RedirectToAction("Error", "Home"); //Shows error page
                    throw;
                }}
        }
    }
}