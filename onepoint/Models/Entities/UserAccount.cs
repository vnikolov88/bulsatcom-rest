using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using onepoint.Models.Providers;

namespace onepoint.Models.Entities
{
    public class UserAccount : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public BulsatcomAccount ProviderAccount { get; set; }
    }
}
