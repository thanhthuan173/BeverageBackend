using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace BeverageBackend.Models
{
    public class BeverageDbContext:DbContext
    {
        public BeverageDbContext(DbContextOptions<BeverageDbContext> options) : base(options)
        {

        }
    }
}
