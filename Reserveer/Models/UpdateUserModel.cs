﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Reserveer.Models
{
    public class UpdateUserModel
    {
        [Required(ErrorMessage = "This field is required.")]
        [MinLength(6, ErrorMessage = "Name has to be longer than 6 Characters.")]
        [MaxLength(20, ErrorMessage = "Name cannot be longer than 20 characters.")]
        public string Namee { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password has to be longer than 6 Characters.")]
        public string Passwordd { get; set; }
    }
}
