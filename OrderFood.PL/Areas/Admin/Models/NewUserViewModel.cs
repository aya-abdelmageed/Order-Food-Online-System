using System.ComponentModel.DataAnnotations;

namespace OrderFood.PL.Areas.Admin.Models
{
    public class NewUserViewModel
    {
        [Required(ErrorMessage ="This Field is Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string LastName { get; set; }
        [Required(ErrorMessage = "This Field is Required")]

        public string Address { get; set; }

        [Required(ErrorMessage ="This Field is required")]
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="You should Select role")]
        
        public string Role { get; set; }

    }
}
