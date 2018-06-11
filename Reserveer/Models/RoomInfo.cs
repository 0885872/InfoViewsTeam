using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Reserveer.Models
{
  public class RoomInfo
    {
      [Required(ErrorMessage = "This field is required.")]
      public string RoomName { get; set; }

      [Required(ErrorMessage = "This field is required.")]
      public string RoomNumber { get; set; }

      [Required(ErrorMessage = "This field is required.")]
      public string RoomFloor { get; set; }

      [Required(ErrorMessage = "This field is required.")]
      public string RoomFacility { get; set; }

      [Required(ErrorMessage = "This field is required.")]
      public string RoomComment { get; set; }
  
      [Required(ErrorMessage = "This field is required.")]
      public string RoomID { get; set; }

      public string SensorID { get; set; }
        public string RoomIDSensor { get; set; }
    }
    public class GroupInfo
    {
        public string GroupName { get; set; }
        public string GroupID { get; set; }
    } 
}
