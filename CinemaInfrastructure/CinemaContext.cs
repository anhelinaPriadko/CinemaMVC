using System;
using CinemaDomain.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

//namespace CinemaDomain.Model;
namespace CinemaInfrastructure;

public partial class CinemaContext : DbContext
{
    public CinemaContext()
    {
    }

    public CinemaContext(DbContextOptions<CinemaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Film> Films { get; set; }

    public virtual DbSet<FilmCategory> FilmCategories { get; set; }

    public virtual DbSet<FilmRating> FilmRatings { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<HallType> HallTypes { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Viewer> Viewers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=4340s\\SQLEXPRESS; Database=cinema; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => new { e.ViewerId, e.SessionId, e.SeatId }).HasName("PK__Bookings__830B0B77C94AE9A3");

            entity.HasOne(d => d.Seat).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__SeatId__5BE2A6F2");

            entity.HasOne(d => d.Session).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Sessio__5AEE82B9");

            entity.HasOne(d => d.Viewer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ViewerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Viewer__59FA5E80");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Companie__3214EC079CF8D54F");

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Film>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Films__6D1D217CEFCE6AF1");

            entity.Property(e => e.Description).HasMaxLength(150);

            entity.Property(e => e.Name)
                  .HasMaxLength(40) // Відповідає nvarchar(40)
                  .IsRequired();

            entity.Property(e => e.PosterPath)
                  .HasMaxLength(255)
                  .IsUnicode(true); 

            entity.HasOne(d => d.Company).WithMany(p => p.Films)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Films__CompanyId__403A8C7D");

            entity.HasOne(d => d.FilmCategory).WithMany(p => p.Films)
                .HasForeignKey(d => d.FilmCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Films__FilmCateg__412EB0B6");
        });

        modelBuilder.Entity<FilmCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FilmCate__3214EC07A5C8E37D");

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<FilmRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FilmRati__3214EC074471E3C0");

            entity.HasOne(d => d.Film).WithMany(p => p.FilmRatings)
                .HasForeignKey(d => d.FilmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FilmRatin__FilmI__48CFD27E");

            entity.HasOne(d => d.Viewer).WithMany(p => p.FilmRatings)
                .HasForeignKey(d => d.ViewerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FilmRatin__Viewe__47DBAE45");
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Halls__7E60E21401F712B2");

            entity.Property(e => e.Name).HasMaxLength(30);

            entity.HasOne(d => d.HallType).WithMany(p => p.Halls)
                .HasForeignKey(d => d.HallTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Halls__HallTypeI__4D94879B");
        });

        modelBuilder.Entity<HallType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HallType__3214EC076C98486F");

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seats__311713F3F7CEAC58");

            entity.HasOne(d => d.Hall).WithMany(p => p.Seats)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seats__HallId__5629CD9C");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sessions__C9F49290B2B6A8E6");

            entity.Property(e => e.SessionTime).HasColumnType("datetime");

            entity.HasOne(d => d.Film).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.FilmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__FilmId__5070F446");

            entity.HasOne(d => d.Hall).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__HallId__5165187F");
        });

        modelBuilder.Entity<Viewer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Viewers__3214EC0789042893");

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
