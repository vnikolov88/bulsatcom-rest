using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace onepoint.Models.Home
{
    public class RegisterModel : LoginModel
    {
        [Required]
        [DisplayName("Confirm Bulsat User")]
        public string nameConfirm { get; set; }

        [Required]
        [DisplayName("Confirm Bulsat Password")]
        public string passwordConfirm { get; set; }

        [DisplayName("API key")]
        public string key { get; set; }
    }
}
