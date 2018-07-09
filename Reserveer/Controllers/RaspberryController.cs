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
            {
                try
                {
                    string room = Request.Query["roomid"];

                    //QRCode Code nog commenten
                    var request = HttpContext.Request;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();

                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://145.24.222.130/Schedule?RoomId=" + room, QRCodeGenerator.ECCLevel.Q);
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

                    string[] temperatureData = db.getLatestTemperature(room);
                    var jsonTemp = JsonConvert.SerializeObject(temperatureData);
                    ViewData["temp"] = jsonTemp;
                    return View();

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Index raspberry Exception: {0}", e);
                    return RedirectToAction("Error", "Home");
                    throw;
                }}
        }
    }
}