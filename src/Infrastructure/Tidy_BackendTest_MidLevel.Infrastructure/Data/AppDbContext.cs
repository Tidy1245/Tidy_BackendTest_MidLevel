using Microsoft.EntityFrameworkCore;
using Tidy_BackendTest_MidLevel.Application.Interfaces;
using Tidy_BackendTest_MidLevel.Domain.Entities;
using Tidy_BackendTest_MidLevel.Domain.Enums;

namespace Tidy_BackendTest_MidLevel.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<MyOfficeAcpd> MyOfficeAcpds { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(e => e.TenantId);

            entity.Property(e => e.TenantId)
                .HasColumnType("char(20)")
                .IsRequired();

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Domain).HasMaxLength(200);

            entity.Property(e => e.SubscriptionStatus)
                .HasColumnType("tinyint")
                .HasDefaultValue(SubscriptionStatus.Trial);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("getdate()");
        });

        modelBuilder.Entity<MyOfficeAcpd>(entity =>
        {
            entity.ToTable("MyOffice_ACPD");
            entity.HasKey(e => e.ACPD_SID);

            entity.Property(e => e.TenantId)
                .HasColumnType("char(20)")
                .IsRequired();

            entity.Property(e => e.ACPD_SID)
                .HasColumnType("char(20)")
                .IsRequired();

            entity.Property(e => e.ACPD_Cname).HasMaxLength(60);
            entity.Property(e => e.ACPD_Ename).HasMaxLength(40);
            entity.Property(e => e.ACPD_Sname).HasMaxLength(40);
            entity.Property(e => e.ACPD_Email).HasMaxLength(60);

            entity.Property(e => e.ACPD_Status)
                .HasColumnType("tinyint")
                .HasDefaultValue((byte)0);

            entity.Property(e => e.ACPD_Stop)
                .HasColumnType("bit")
                .HasDefaultValue(false);

            entity.Property(e => e.ACPD_StopMemo).HasMaxLength(60);
            entity.Property(e => e.ACPD_LoginID).HasMaxLength(30);
            entity.Property(e => e.ACPD_LoginPWD).HasMaxLength(60);
            entity.Property(e => e.ACPD_Memo).HasMaxLength(600);
            entity.Property(e => e.ACPD_NowID).HasMaxLength(20);
            entity.Property(e => e.ACPD_UPDID).HasMaxLength(20);

            entity.Property(e => e.ACPD_NowDateTime)
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.ACPD_UPDDateTime)
                .HasDefaultValueSql("getdate()");

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e =>
                _tenantContext.TenantId != null &&
                e.TenantId == _tenantContext.TenantId);
        });
    }
}
