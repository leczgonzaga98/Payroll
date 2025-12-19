using Microsoft.AspNetCore.Mvc;
using Sample.Models;
using Sample.Data;

namespace Sample.Controllers
{
    public class PayrollController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Payroll payroll)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            payroll.CreatedAt = DateTime.Now;
            payroll.EmployeeNumber = Utilities.Generate(payroll.EmployeeName, 
                Utilities.ConvertDateTime(payroll.DateOfBirth));

            _context.Payrolls.Add(payroll);
            _context.SaveChanges();

            return Ok(new { message = "Created successfully" });
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] Payroll updatedPayroll)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payroll = _context.Payrolls.Find(id);
            if (payroll == null)
                return NotFound(new { message = "Payroll record not found" });

            // Update fields
            payroll.EmployeeName = updatedPayroll.EmployeeName;
            payroll.DateOfBirth = updatedPayroll.DateOfBirth;
            payroll.DailyRate = updatedPayroll.DailyRate;
            payroll.WorkingDays = updatedPayroll.WorkingDays;

            _context.SaveChanges();

            return Ok(new { message = "Updated successfully" });
        }
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var payroll = _context.Payrolls.Find(id);
            if (payroll == null)
                return NotFound(new { message = "Payroll record not found" });

            _context.Payrolls.Remove(payroll);
            _context.SaveChanges();

            return Ok(new { message = "Deleted successfully" });
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            var employees = _context.Payrolls
                .AsEnumerable() // switch to LINQ-to-Objects
                .Select(e =>
                {
                    var week = Utilities.GetPatternWeek(Utilities.ConvertDateTime(e.DateOfBirth), e.WorkingDays);
                    var takeHomePay = (e.DailyRate * week.DaysInPattern) * 2;
                    return new EmployeeWeekDto
                    {
                        Id = e.Id,
                        EmployeeNumber = e.EmployeeNumber,
                        EmployeeName = e.EmployeeName,
                        DateOfBirth = Utilities.ConvertDateTime(e.DateOfBirth),
                        WorkingDays = e.WorkingDays,
                        StartDate = week.StartDate,
                        EndDate = week.EndDate,
                        TakeHomePay = takeHomePay + e.DailyRate
                    };
                })
                .ToList();

            return Ok(employees);
        }


    }
}
