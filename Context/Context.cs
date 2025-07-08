using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SportCentrum.Models;

namespace SportCentrum.Context
{
    public class SportCentrumContext : DbContext
    {
        public SportCentrumContext(DbContextOptions<SportCentrumContext> options) :base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<TrainingSession> Sessions { get; set; }
        public DbSet<TrainingReservation> Reservations { get; set; }
        public DbSet<UserTraining> UserTrainings { get; set; }
        public DbSet<CoachTraining> CoachTrainings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTraining>().HasKey(ut => new { ut.UserId, ut.TrainingId });
            modelBuilder.Entity<UserTraining>().HasOne(ut => ut.User).WithMany(u => u.UserTrainings).HasForeignKey(ut => ut.UserId);
            modelBuilder.Entity<UserTraining>().HasOne(ut => ut.Training).WithMany(t => t.UserTrainings).HasForeignKey(ut => ut.TrainingId);

            modelBuilder.Entity<TrainingSession>().HasOne(ts => ts.Coach).WithMany(c => c.Sessions).HasForeignKey(ts => ts.CoachId).IsRequired(false);
            modelBuilder.Entity<TrainingSession>().HasOne(ts => ts.Training).WithMany(t => t.Sessions).HasForeignKey(ts => ts.TrainingId);

            modelBuilder.Entity<TrainingReservation>().HasOne(tr => tr.User).WithMany(u => u.Reservations).HasForeignKey(tr =>  tr.UserId);
            modelBuilder.Entity<TrainingReservation>().HasOne(tr => tr.TrainingSession).WithMany(ts => ts.Reservations).HasForeignKey(tr => tr.TrainingSessionId);

            modelBuilder.Entity<CoachTraining>().HasKey(ct => new { ct.CoachId, ct.TrainingId });
            modelBuilder.Entity<CoachTraining>().HasOne(ct => ct.Coach).WithMany(c => c.CoachTrainings).HasForeignKey(ct => ct.CoachId);
            modelBuilder.Entity<CoachTraining>().HasOne(ct => ct.Training).WithMany(t => t.CoachTrainings).HasForeignKey(ct => ct.TrainingId);

        }
    }
}