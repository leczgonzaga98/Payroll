using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Models
{
    public class EmployeeWeekDto
    {
        public int Id { get; set; }
        public string? EmployeeNumber { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string WorkingDays { get; set; }
        public Decimal TakeHomePay { get; set; }
    }

}
