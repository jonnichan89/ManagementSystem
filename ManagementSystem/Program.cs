using ManagementSystem;
using ManagementSystem.DataServices.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB in bytes
});

builder.Services.AddSingleton(typeof(ManagementSystem.DataServices.Resources.SharedResources));

// Add services to the container.
builder.Services.AddScoped<MstUserService>();
builder.Services.AddScoped<MstItemService>();
builder.Services.AddScoped<MstSupplierService>();
builder.Services.AddScoped<MstAccountService>();
builder.Services.AddScoped<ProjectTemplateService>();
builder.Services.AddScoped<ItemUsageService>();
builder.Services.AddScoped<PurchaseOrderService>();
builder.Services.AddScoped<SalesOrderService>();
builder.Services.AddScoped<ProjectPurchaseOrderService>();
builder.Services.AddScoped<ProjectSalesOrderService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<ItemCalcService>();
builder.Services.AddScoped<ItemAdjustmentService>();

builder.Services
    .AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new EmptyStringModelBinderProvider());
        options.AllowEmptyInputInBodyModelBinding = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = null; // ReferenceHandler.Preserve;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1. Add Localization Services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads", "mstitems")),
    RequestPath = "/MstItems"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads", "projectTemplates")),
    RequestPath = "/ProjectTemplates"
});

//app.MapGet("/uploads/{folderName}/{fileName}", (string folderName, string fileName, IWebHostEnvironment env) =>
//{
//    try
//    {
//        // Define the secure internal path
//        var filePath = Path.Combine(env.WebRootPath, "uploads", folderName, fileName);
//        // Check if the file actually exists internally
//        if (!File.Exists(filePath))
//        {
//            return Results.NotFound(); // Returns a direct 404
//        }

//        // Stream the internal file directly to the client
//        var returnType = "image/" + System.IO.Path.GetExtension(fileName).Replace(".", "");
//        return Results.File(filePath, returnType);
//    }
//    catch (HttpRequestException)
//    {
//        // Automatically returns 404 if the external fetch fails
//        return Results.NotFound();
//    }
//});

// 2. Configure Supported Cultures
var supportedCultures = new[] { "en-US", "id-ID" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// 3. Enable the Middleware
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
