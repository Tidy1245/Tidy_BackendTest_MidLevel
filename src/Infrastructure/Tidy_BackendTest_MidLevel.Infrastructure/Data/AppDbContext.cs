using Microsoft.EntityFrameworkCore;
using Tidy_BackendTest_MidLevel.Domain.Entities;

namespace Tidy_BackendTest_MidLevel.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MyOfficeAcpd> MyOfficeAcpds { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyOfficeAcpd>(entity =>
        {
            entity.ToTable("MyOffice_ACPD");
            entity.HasKey(e => e.ACPD_SID);

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
        });
    }
}
