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

    public class RegisterModel : LoginModel
    {
        [Required]
        [DisplayName("Confirm Bulsat User")]
        public string nameConfirm { get; set; }

        [Required]
        [DisplayName("Confirm Bulsat Password")]
        public string passwordConfirm { get; set; }

        [DisplayName("API KEY")]
        public string key { get; set; }
    }
}
