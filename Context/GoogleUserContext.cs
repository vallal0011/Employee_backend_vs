using Microsoft.EntityFrameworkCore;
using asp_net.Models;

namespace asp_net.Context
{
    public class GoogleUserContext : DbContext
    {
        public GoogleUserContext(DbContextOptions<GoogleUserContext> options) : base(options)
        {
        }

        public DbSet<GoogleUser> GoogleUsers { get; set; }
    }
}
