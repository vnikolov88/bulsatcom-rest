using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace onepoint.Models.Home
{
    public class LoginModel
    {
        [Required]
        [DisplayName("Bulsat User")]
        public string name { get; set; }

        [Required]
        [DisplayName("Bulsat Password")]
        public string password { get; set; }
    }
}
