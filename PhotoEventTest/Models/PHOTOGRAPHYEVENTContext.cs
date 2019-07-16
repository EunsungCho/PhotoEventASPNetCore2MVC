using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PhotoEventTest.Models
{
    public partial class PHOTOGRAPHYEVENTContext : DbContext
    {
        public PHOTOGRAPHYEVENTContext()
        {
        }

        public PHOTOGRAPHYEVENTContext(DbContextOptions<PHOTOGRAPHYEVENTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EventUserPhotos> EventUserPhotos { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=PHOTOGRAPHYEVENT;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<EventUserPhotos>(entity =>
            {
                entity.HasKey(e => new { e.EventId, e.UserId })
                    .HasName("PK_Table");

                entity.Property(e => e.UserId).HasMaxLength(50);

                entity.Property(e => e.PhotoTitle).HasMaxLength(1000);

                entity.Property(e => e.PhotoUploadDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserIdToVote).HasMaxLength(50);

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventUserPhotos)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventUserPhotos_Events");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.EventUserPhotos)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventUserPhotos_Users");
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.HasKey(e => e.EventId)
                    .HasName("PK__tmp_ms_x__7944C810C5678DB0");

                entity.Property(e => e.EndDate)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EventName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.EventRule).HasColumnType("ntext");

                entity.Property(e => e.IsClosed).HasDefaultValueSql("((0))");

                entity.Property(e => e.StartDate)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Winner).HasMaxLength(50);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Users__1788CC4C5D647209");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.EntryDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.IsAdmin).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastName).HasMaxLength(10);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
        }
    }
}
