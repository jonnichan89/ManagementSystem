using ManagementSystem;
using ManagementSystem.DataServices.Service;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
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
builder.Services.AddScoped<ItemUsageService>();
builder.Services.AddScoped<PurchaseOrderService>();

builder.Services
    .AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new EmptyStringModelBinderProvider());
        options.AllowEmptyInputInBodyModelBinding = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1. Add Localization Services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads", "items")),
    RequestPath = "/items"
});

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
