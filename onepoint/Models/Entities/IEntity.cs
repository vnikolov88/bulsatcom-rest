using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace onepoint.Models.Entities
{
    public interface IEntity
    {
        [Key]
        Guid Id { get; set; }
    }
}
