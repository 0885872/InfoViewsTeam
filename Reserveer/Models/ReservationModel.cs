using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reserveer.Models;
using Newtonsoft.Json;
using Reserveer.Controllers;

namespace Reserveer.Models
{

    public class ReservationModel // Model for reservation page
    {
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string roomid { get; set; }
        public string userid { get; set; }
    }
}