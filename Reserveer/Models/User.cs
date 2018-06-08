using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Reserveer.Models
{
    public class User
    {
        [Key]
        public int group_id { get; set; }

        public string user_name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name="Password: ")]
        public string user_password { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email adress: ")]
        public string user_mail { get; set; }

        public string password_salt { get; set; }

        public string user_role { get; set; }
    }
}
