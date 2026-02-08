namespace ManagementSystem.Data.DBContext;

public partial class ManagementSystemContext
{
    private string _connectionString = "";
    public ManagementSystemContext()
        : this(ConfigurationHelper.GetConnectionString("ConnStr"))
    {
    }

    public ManagementSystemContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //// 2 syntax dibawah jangan di-comment, ini digunakan untuk keperluan dotnet ef database di package manager console.
        //if (string.IsNullOrWhiteSpace(_connectionString))
        //{
        //    IConfigurationRoot configuration = new ConfigurationBuilder()
        //        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

        //    _connectionString = configuration.GetConnectionString("ConnStr");
        //}

        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            _connectionString = ConfigurationHelper.GetConnectionString("ConnStr") ?? "";
        }

        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Database connection string has not been configured yet.");
        }

        _ = optionsBuilder.UseSqlServer(
            _connectionString,
            ob => ob.MigrationsHistoryTable("__EFMigrationsHistory", "dbo")
        );

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    }
}
