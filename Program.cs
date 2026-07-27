using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using BillingSystem.Data;
using BillingSystem.Data.Repositories;
using BillingSystem.Mappings;
using BillingSystem.Middleware;
using BillingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPartyRepository, PartyRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerProductPriceRepository, CustomerProductPriceRepository>();
builder.Services.AddScoped<IBillRepository, BillRepository>();

builder.Services.AddScoped<IPartyService, PartyService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerProductPriceService, CustomerProductPriceService>();
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<IPdfService, PdfService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Billing System API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ... after builder.Build()


var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";




var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Billing System API v1");
    c.RoutePrefix = string.Empty;
});

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors("ReactPolicy");

app.MapControllers();

app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
