using Microsoft.EntityFrameworkCore;
using PetCare.Shared;

namespace PetCare.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Appointment> Appointments {  get; set; }
        public DbSet <CareLog> CareLogs { get; set; }
        public DbSet <Medication> Medication { get; set; }
        public DbSet<PetSharedAccess> PetsSharedAccess { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
    }
}
