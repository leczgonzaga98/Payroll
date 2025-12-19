using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Models
{
    [Table("payroll")]
    public class Payroll
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("employee_number")]
        [StringLength(100)]
        public string? EmployeeNumber { get; set; }

        [Required]
        [Column("employee_name")]
        [StringLength(100)]
        public required string EmployeeName { get; set; }

        [Required]
        [Column("date_of_birth")]
        public required string DateOfBirth { get; set; }

        [Required]
        [Column("daily_rate", TypeName = "decimal(8,2)")]
        public decimal DailyRate { get; set; }

        [Required]
        [Column("working_days")]
        public required string WorkingDays { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
