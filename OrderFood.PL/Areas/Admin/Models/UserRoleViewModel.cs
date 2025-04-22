using OrderFood.DAL.Entities.User;

namespace OrderFood.PL.Areas.Admin.Models
{
    public class UserRoleViewModel
    {
        public ApplicationUser user { get; set; }
        public string role { get; set; }
    }
}
