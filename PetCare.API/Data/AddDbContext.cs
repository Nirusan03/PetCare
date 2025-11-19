using Microsoft.EntityFrameworkCore;
using PetCare.Shared;

namespace PetCare.API.Data
{
    public class AddDbContext : DbContext
    {
        public AddDbContext(DbContextOptions<AddDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Appointment> Appointments {  get; set; }
        public DbSet <CareLog> CareLogs { get; set; }
        public DbSet <Medication> Medication { get; set; }
        public DbSet<PetSharedAccess> PetsSharedAccess { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // FIX: Prevent "multiple cascade paths" error in SQL Server.
            // When a Pet is deleted, its Appointments should be deleted (Cascade).
            // When a User is deleted, we must explicitly prevent the cascade on direct
            // relationships (like User -> Appointment) to avoid ambiguity.
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<CareLog>()
                .HasOne(cl => cl.User)
                .WithMany()
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PetSharedAccess>()
                .HasOne(psa => psa.Owner)
                .WithMany()
                .HasForeignKey(psa => psa.OwnerUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
