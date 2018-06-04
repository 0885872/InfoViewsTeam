using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InfoViewsProject.Models;
using Newtonsoft.Json;
using Reserveer.Controllers;

namespace InfoViewsProject.Models
{

    public class ReservationModel
    {
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }
}