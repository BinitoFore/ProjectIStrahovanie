using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CurseProject.Models
{
    public class DataBaseContext: IdentityDbContext<User>
    {
        
        public DbSet<User> Users { get; set; }

        public DbSet<IdentityRole> Roles { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<Property> properties { get; set; }
        public DbSet<InsAmenities> insAmenities { get; set; }
        public DbSet<Req_for_paym> req_For_Payms { get; set; }
        public DbSet<Legal_entity> legal_entity { get; set; }
        public DbSet<Risk> riscs { get; set; }
        public DataBaseContext(DbContextOptions<DataBaseContext> options): base(options)
        {
            
            Database.EnsureCreated();

            AddRange(new IdentityRole{Name = "Admin", NormalizedName = "ADMIN"},
                new IdentityRole { Name = "Agent", NormalizedName = "AGENT"}, 
                new IdentityRole { Name = "Client", NormalizedName = "CLIENT"}
            );

            SaveChanges();

            

        }
    }
}
