using Microsoft.EntityFrameworkCore;
using CandidateManagement.Api.Models; 

namespace CandidateManagement.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Candidate> Candidates { get; set; } 
        public DbSet<Skill> Skills { get; set; } 
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; } 
        public DbSet<OutboxMessage> OutboxMessages { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("citext");

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.FullName)
                    .IsRequired()
                    .HasColumnType("citext")
                    .HasMaxLength(50);

                entity.Property(c => c.DateOfBirth)
                    .IsRequired();

                entity.Property(c => c.ContactNumber)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(c => c.EmailAddress)
                    .IsRequired()
                    .HasColumnType("citext")
                    .HasMaxLength(50);

                entity.HasIndex(c => c.EmailAddress)
                    .IsUnique();
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.SkillName)
                    .IsRequired()
                    .HasColumnType("citext")
                    .HasMaxLength(50);

                entity.HasIndex(s => s.SkillName)
                    .IsUnique();
            });

            modelBuilder.Entity<CandidateSkill>(entity =>
            {
                entity.HasKey(cs => new { cs.CandidateId, cs.SkillId });

                entity.HasOne(cs => cs.Candidate)
                    .WithMany(c => c.CandidateSkills)
                    .HasForeignKey(cs => cs.CandidateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cs => cs.Skill)
                    .WithMany(s => s.CandidateSkills)
                    .HasForeignKey(cs => cs.SkillId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.EventId)
                    .IsRequired();

                entity.Property(a => a.EventType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(a => a.EntityType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(a => a.EntityId)
                    .IsRequired();

                entity.Property(a => a.EntityName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.Message)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(a => a.OccurredAtUtc)
                    .IsRequired();

                entity.HasIndex(a => a.OccurredAtUtc);

                entity.HasIndex(a => a.EventId)
                    .IsUnique();

            });

            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.EventId)
                    .IsRequired();

                entity.Property(o => o.EventType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(o => o.Payload)
                    .IsRequired();

                entity.Property(o => o.OccurredAtUtc)
                    .IsRequired();

                entity.Property(o => o.ProcessedAtUtc);

                entity.Property(o => o.RetryCount)
                    .IsRequired();

                entity.Property(o => o.Error)
                    .HasMaxLength(1000);

                entity.HasIndex(o => o.EventId)
                    .IsUnique();

                entity.HasIndex(o => o.ProcessedAtUtc);

                entity.HasIndex(o => o.OccurredAtUtc);
            });
        }
    }
}
