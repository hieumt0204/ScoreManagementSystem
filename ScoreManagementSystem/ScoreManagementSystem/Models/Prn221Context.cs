using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ScoreManagementSystem.Models;

public partial class Prn221Context : DbContext
{
    public Prn221Context()
    {
    }

    public Prn221Context(DbContextOptions<Prn221Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassStudent> ClassStudents { get; set; }

    public virtual DbSet<ComponentScore> ComponentScores { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DbConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Class");

            entity.Property(e => e.CreatedAt).HasColumnType("date");
            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ClassCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Class_User1");

            entity.HasOne(d => d.Subject).WithMany(p => p.Classes)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_Class_Subject");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ClassTeachers)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Class_User");
        });

        modelBuilder.Entity<ClassStudent>(entity =>
        {
            entity.ToTable("ClassStudent");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JoinDate).HasColumnType("date");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_ClassStudent_Class");

            entity.HasOne(d => d.Student).WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_ClassStudent_User");
        });

        modelBuilder.Entity<ComponentScore>(entity =>
        {
            entity.ToTable("ComponentScore");

            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasOne(d => d.Subject).WithMany(p => p.ComponentScores)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_ComponentScore_Subject");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_StudentScore");

            entity.ToTable("Score");

            entity.Property(e => e.Note).HasMaxLength(250);
            entity.Property(e => e.Score1).HasColumnName("Score");

            entity.HasOne(d => d.ComponentScore).WithMany(p => p.Scores)
                .HasForeignKey(d => d.ComponentScoreId)
                .HasConstraintName("FK_Score_ComponentScore");

            entity.HasOne(d => d.Student).WithMany(p => p.Scores)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Score_ClassStudent");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subject");

            entity.Property(e => e.CreatedAt).HasColumnType("date");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Subject_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
