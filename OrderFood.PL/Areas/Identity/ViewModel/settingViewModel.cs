using System.ComponentModel.DataAnnotations;

namespace OrderFood.PL.Areas.Identity.ViewModel
{
    public class settingViewModel
    {

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Address")]
            public string Address { get; set; }

            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateTime? DateOfBirth { get; set; }
            public IFormFile? Image { get; set; }



    }
}
