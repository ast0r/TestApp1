using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestApp1.Models
{
    public class TicketStatus
    {
        [Key]
        public int StatusId { get; set; }

        [MaxLength(50)]
        public string StatusTitle { get; set; }
    }
}