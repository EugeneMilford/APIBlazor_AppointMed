using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointMed.API.Data
{
    public partial class AppointMedDbContext : IdentityDbContext<ApiUser>
    {
        public AppointMedDbContext()
        {
        }

        public AppointMedDbContext(DbContextOptions<AppointMedDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Medicine> Medicines { get; set; }
        public virtual DbSet<Prescription> Prescriptions { get; set; }
        public virtual DbSet<AccountTransaction> AccountTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC227ECFDA1");

                entity.Property(e => e.UserId).IsRequired();

                entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");
                entity.Property(e => e.AppointmentType).HasMaxLength(100);
                entity.Property(e => e.CancellationReason).HasMaxLength(500);
                entity.Property(e => e.CancelledAt).HasColumnType("datetime");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.PatientEmail).HasMaxLength(100);
                entity.Property(e => e.PatientFirstName).HasMaxLength(50);
                entity.Property(e => e.PatientLastName).HasMaxLength(50);
                entity.Property(e => e.PatientPhoneNumber).HasMaxLength(15);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appointments_Doctors");

                entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appointments_Statuses");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appointments_Users");
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EBF99B922EB");

                entity.HasIndex(e => e.Email, "UQ__Doctors__A9D10534C6805655").IsUnique();

                entity.Property(e => e.Bio).HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateJoined)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.Property(e => e.ProfileImageUrl).HasMaxLength(255);
                entity.Property(e => e.Specialization).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(e => e.StatusId).HasName("PK__Statuses__C8EE20637C2AE338");

                entity.HasIndex(e => e.StatusName, "UQ__Statuses__05E7698A6745702A").IsUnique();

                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.StatusDescription).HasMaxLength(200);
                entity.Property(e => e.StatusName).HasMaxLength(50);
            });

            // Account Configuration
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccountId);

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithOne()
                    .HasForeignKey<Account>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Accounts_Users");

                entity.HasIndex(e => e.UserId).IsUnique();
            });

            // Medicine Configuration
            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.HasKey(e => e.MedicineId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Dosage)
                    .HasMaxLength(100);

                entity.Property(e => e.Manufacturer)
                    .HasMaxLength(200);

                entity.Property(e => e.IsAvailable)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime");
            });

            // Prescription Configuration
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(e => e.PrescriptionId);

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.Instructions)
                    .HasMaxLength(1000);

                entity.Property(e => e.PrescribedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.IsFulfilled)
                    .HasDefaultValue(false);

                entity.Property(e => e.FulfilledDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Appointment)
                    .WithMany()
                    .HasForeignKey(d => d.AppointmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prescriptions_Appointments");

                entity.HasOne(d => d.Medicine)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.MedicineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prescriptions_Medicines");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prescriptions_Users");
            });

            // AccountTransaction Configuration
            modelBuilder.Entity<AccountTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.TransactionDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AccountTransactions_Accounts");

                entity.HasOne(d => d.Appointment)
                    .WithMany()
                    .HasForeignKey(d => d.AppointmentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_AccountTransactions_Appointments");

                entity.HasOne(d => d.Prescription)
                    .WithMany()
                    .HasForeignKey(d => d.PrescriptionId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_AccountTransactions_Prescriptions");
            });

            // Seeding Roles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    Id = "111"
                },
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    Id = "112"
                }
            );

            var hasher = new PasswordHasher<ApiUser>();

            // Seeding Users 
            modelBuilder.Entity<ApiUser>().HasData(
                new ApiUser
                {
                    Id = "113",
                    Email = "admin@appointmed.com",
                    NormalizedEmail = "ADMIN@APPOINTMED.COM",
                    UserName = "admin@appointmed.com",
                    NormalizedUserName = "ADMIN@APPOINTMED.COM",
                    FirstName = "System",
                    LastName = "Admin",
                    Address = "Cape Town",
                    Role = "Administrator",
                    PasswordHash = hasher.HashPassword(null, "Admin@123"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApiUser
                {
                    Id = "114",
                    Email = "user@appointmed.com",
                    NormalizedEmail = "USER@APPOINTMED.COM",
                    UserName = "user@appointmed.com",
                    NormalizedUserName = "USER@APPOINTMED.COM",
                    FirstName = "System",
                    LastName = "User",
                    Address = "Cape Town",
                    Role = "User",
                    PasswordHash = hasher.HashPassword(null, "User@123"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            );

            // Seeding User Roles
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "111",
                    UserId = "114"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "112",
                    UserId = "113"
                }
            );

            // Seeding Statuses
            modelBuilder.Entity<Status>().HasData(
                new Status
                {
                    StatusId = 1,
                    StatusName = "Scheduled",
                    StatusDescription = "Appointment has been scheduled",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new Status
                {
                    StatusId = 2,
                    StatusName = "Confirmed",
                    StatusDescription = "Patient has confirmed the appointment",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new Status
                {
                    StatusId = 3,
                    StatusName = "Completed",
                    StatusDescription = "Appointment has been completed",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new Status
                {
                    StatusId = 4,
                    StatusName = "Cancelled",
                    StatusDescription = "Appointment has been cancelled",
                    DisplayOrder = 4,
                    IsActive = true
                },
                new Status
                {
                    StatusId = 5,
                    StatusName = "No Show",
                    StatusDescription = "Patient did not show up for appointment",
                    DisplayOrder = 5,
                    IsActive = true
                }
            );

            // Seeding Medicine
            modelBuilder.Entity<Medicine>().HasData(
                new Medicine
                {
                    MedicineId = 1,
                    Name = "Amoxicillin",
                    Description = "Antibiotic used to treat bacterial infections",
                    Price = 45.00m,
                    Dosage = "500mg",
                    Manufacturer = "PharmaCorp",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    MedicineId = 2,
                    Name = "Ibuprofen",
                    Description = "Pain reliever and anti-inflammatory",
                    Price = 20.00m,
                    Dosage = "200mg",
                    Manufacturer = "HealthMeds",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    MedicineId = 3,
                    Name = "Paracetamol",
                    Description = "Pain and fever reducer",
                    Price = 15.00m,
                    Dosage = "500mg",
                    Manufacturer = "MediPlus",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    MedicineId = 4,
                    Name = "Omeprazole",
                    Description = "Treats stomach acid and heartburn",
                    Price = 35.00m,
                    Dosage = "20mg",
                    Manufacturer = "GastroMed",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Medicine
                {
                    MedicineId = 5,
                    Name = "Cetirizine",
                    Description = "Antihistamine for allergies",
                    Price = 25.00m,
                    Dosage = "10mg",
                    Manufacturer = "AllergyFree",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
