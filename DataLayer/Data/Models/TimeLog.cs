namespace DataLayer.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    public class TimeLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0.25, 8.00)]
        public decimal Hours { get; set; }
    }
}
