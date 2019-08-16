using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TestApp1.Models
{
    public class TicketModel
    {
        [Key]
        public int TicketId { get; set; }

        [Display(Name = "Customer Name")]
        [Required]
        [MaxLength(24)]
        public string CustomerName { get; set; }

        [EmailAddress]
        [Required]
        [MaxLength(50)]
        [Display(Name = "Email")]
        public string Email { get; set; }


        
        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Text")]
        public string TicketText { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? Time { get; set; }

        public string GUIDLink { get; set; }
        public int? StatusId { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }

        [Display(Name = "Owner")]
        public string OwnerID { get; set; }        

        [MaxLength(100)]
        [DataType(DataType.MultilineText)]
        public string Answer { get; set; }
    }
}