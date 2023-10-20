using LostButFound.API.Domian;
using Microsoft.EntityFrameworkCore;

namespace LostButFound.API.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Thing> Things { get; set; }

    }
}
