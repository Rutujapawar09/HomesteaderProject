using Microsoft.EntityFrameworkCore;
using Template.Models;

namespace Template.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
