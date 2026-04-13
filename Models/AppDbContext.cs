using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InsuranceClaimsApi.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuthUser> AuthUsers { get; set; }

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("auth_user_pkey");

            entity.Property(e => e.CreateDt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdateDt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.ToTable("policy");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PolicyNumber).HasColumnName("policy_number");
            entity.Property(e => e.CustomerName).HasColumnName("customer_name");
            entity.Property(e => e.PolicyType).HasColumnName("policy_type");
            entity.Property(e => e.CoverageAmount).HasColumnName("coverage_amount");
            entity.Property(e => e.PremiumAmount).HasColumnName("premium_amount");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CreateDt)
                .HasColumnName("create_dt")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    //Policy Register
    public virtual DbSet<Policy> Policies { get; set; }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
        

    //     OnModelCreatingPartial(modelBuilder);
    // }

    // partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
