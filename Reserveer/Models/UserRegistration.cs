using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Reserveer.Models
{
    public class UserRegistration // Models to save data in user registration
    {

    [Required(ErrorMessage = "This field is required.")]
    [MinLength(6, ErrorMessage = "Name has to be longer than 6 Characters.")]
    [MaxLength(20, ErrorMessage = "Name cannot be longer than 20 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password has to be longer than 6 Characters.")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password has to be longer than 6 Characters.")]
    [DisplayName("Confirm Password")]
    [Compare("Password")]
    public string ConfirmPass { get; set; }

    [Required(ErrorMessage = "This field is required.")]
    [EmailAddress]
    public string Mail { get; set; }
  }
}
