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

    public virtual DbSet<ItemAdjustment> ItemAdjustments { get; set; }

    public virtual DbSet<ItemAdjustmentDetail> ItemAdjustmentDetails { get; set; }

    public virtual DbSet<ItemCalc> ItemCalcs { get; set; }

    public virtual DbSet<ItemCalcDetail> ItemCalcDetails { get; set; }

    public virtual DbSet<ItemUsage> ItemUsages { get; set; }

    public virtual DbSet<ItemUsageDetail> ItemUsageDetails { get; set; }

    public virtual DbSet<MstAccount> MstAccounts { get; set; }

    public virtual DbSet<MstItem> MstItems { get; set; }

    public virtual DbSet<MstSupplier> MstSuppliers { get; set; }

    public virtual DbSet<MstUser> MstUsers { get; set; }

    public virtual DbSet<PSOrder> PSOrders { get; set; }

    public virtual DbSet<PSOrderDetail> PSOrderDetails { get; set; }

    public virtual DbSet<PSOrderPT> PSOrderPTs { get; set; }

    public virtual DbSet<ProjectTemplate> ProjectTemplates { get; set; }

    public virtual DbSet<ProjectTemplateDetail> ProjectTemplateDetails { get; set; }

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

        modelBuilder.Entity<ItemAdjustmentDetail>(entity =>
        {
            entity.Property(e => e.IADId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ItemAdjustment).WithMany(p => p.ItemAdjustmentDetails).HasConstraintName("FK_ItemAdjustmentDetail_ItemAdjustment");
        });

        modelBuilder.Entity<ItemCalc>(entity =>
        {
            entity.Property(e => e.AfterAmount).HasDefaultValue(0m);
            entity.Property(e => e.AfterQty).HasDefaultValue(0m);
            entity.Property(e => e.AfterUP).HasDefaultValue(0m);
            entity.Property(e => e.BeforeAmount).HasDefaultValue(0m);
            entity.Property(e => e.BeforeQty).HasDefaultValue(0m);
            entity.Property(e => e.BeforeUP).HasDefaultValue(0m);
            entity.Property(e => e.CalcAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IAQty).HasDefaultValue(0m);
            entity.Property(e => e.IUQty).HasDefaultValue(0m);
            entity.Property(e => e.InAmount).HasDefaultValue(0m);
            entity.Property(e => e.InQty).HasDefaultValue(0m);
            entity.Property(e => e.OutAmount).HasDefaultValue(0m);
            entity.Property(e => e.OutQty).HasDefaultValue(0m);
            entity.Property(e => e.POQty).HasDefaultValue(0m);
            entity.Property(e => e.SOQty).HasDefaultValue(0m);
            entity.Property(e => e.SPQty).HasDefaultValue(0m);
        });

        modelBuilder.Entity<ItemUsageDetail>(entity =>
        {
            entity.Property(e => e.ItemUsageDetailId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ItemUsage).WithMany(p => p.ItemUsageDetails).HasConstraintName("FK_ItemUsageDetail_ItemUsage");
        });

        modelBuilder.Entity<MstItem>(entity =>
        {
            entity.Property(e => e.Colour).HasDefaultValue("");
            entity.Property(e => e.SupplierCode).HasDefaultValue("");
        });

        modelBuilder.Entity<PSOrderDetail>(entity =>
        {
            entity.HasOne(d => d.PSOrder).WithMany(p => p.PSOrderDetails).HasConstraintName("FK_PSOrderDetail_PSOrder");
        });

        modelBuilder.Entity<PSOrderPT>(entity =>
        {
            entity.HasOne(d => d.PSOrder).WithMany(p => p.PSOrderPTs).HasConstraintName("FK_PSOrderPT_PSOrder");
        });

        modelBuilder.Entity<ProjectTemplate>(entity =>
        {
            entity.HasOne(d => d.ParentProjectTemplate).WithMany(p => p.ProjectTemplates).HasConstraintName("FK_ProjectTemplate_ProjectTemplate");
        });

        modelBuilder.Entity<ProjectTemplateDetail>(entity =>
        {
            entity.HasKey(e => new { e.PTId, e.PTDId }).HasName("PK_ProjectTemplateDetail_1");

            entity.HasOne(d => d.ProjectTemplate).WithMany(p => p.ProjectTemplateDetails).HasConstraintName("FK_ProjectTemplateDetail_ProjectTemplate");
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
