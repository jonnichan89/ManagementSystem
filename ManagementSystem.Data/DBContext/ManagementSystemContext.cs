using ManagementSystem.Data.Models;

namespace ManagementSystem.Data.DBContext;

public partial class ManagementSystemContext : DbContext
{
    public ManagementSystemContext(DbContextOptions<ManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Finance> Finances { get; set; }

    public virtual DbSet<FinanceDetail> FinanceDetails { get; set; }

    public virtual DbSet<ItemUsage> ItemUsages { get; set; }

    public virtual DbSet<ItemUsageDetail> ItemUsageDetails { get; set; }

    public virtual DbSet<MstAccount> MstAccounts { get; set; }

    public virtual DbSet<MstItem> MstItems { get; set; }

    public virtual DbSet<MstSupplier> MstSuppliers { get; set; }

    public virtual DbSet<MstUser> MstUsers { get; set; }

    public virtual DbSet<PSOrder> PSOrders { get; set; }

    public virtual DbSet<PSOrderDetail> PSOrderDetails { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<StockMovement> StockMovements { get; set; }

    public virtual DbSet<StockSummary> StockSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FinanceDetail>(entity =>
        {
            entity.HasOne(d => d.Finance).WithMany(p => p.FinanceDetails).HasConstraintName("FK_FinanceDetail_Finance");
        });

        modelBuilder.Entity<ItemUsageDetail>(entity =>
        {
            entity.Property(e => e.ItemUsageDetailId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ItemUsage).WithMany(p => p.ItemUsageDetails).HasConstraintName("FK_ItemUsageDetail_ItemUsage");
        });

        modelBuilder.Entity<MstItem>(entity =>
        {
            entity.Property(e => e.SupplierCode).HasDefaultValue("");
        });

        modelBuilder.Entity<PSOrderDetail>(entity =>
        {
            entity.HasOne(d => d.PSOrder).WithMany(p => p.PSOrderDetails).HasConstraintName("FK_PSOrderDetail_PSOrder");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderDetails).HasConstraintName("FK_PurchaseOrderDetail_PurchaseOrder");
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.Property(e => e.SMId).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
