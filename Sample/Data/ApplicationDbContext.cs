using Microsoft.EntityFrameworkCore;
using Sample.Models;

namespace Sample.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
                    
        }

        public DbSet<Payroll> Payrolls { get; set; }
    }
}
