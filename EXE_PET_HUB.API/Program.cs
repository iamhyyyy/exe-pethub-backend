using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Mappings;
using EXE_PET_HUB.Application.Services;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using EXE_PET_HUB.Infrastructure.Repositories;
using EXE_PET_HUB.Infrastructure.Services;
using EXE_PET_HUB.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EXE_PET_HUB.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure email settings
            builder.Services.Configure<SendGridSettings>(
            builder.Configuration.GetSection("SendGridSettings"));

            
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<PetService>();
            builder.Services.AddScoped<IPetRepository, PetRepository>();

            builder.Services.AddScoped<MedicalRecordService>();
            builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();

            builder.Services.AddScoped<ItemService>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();

            builder.Services.AddScoped<AppointmentService>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            //builder.Services.AddScoped<AppointmentReminderService>();
            builder.Services.AddScoped<IAppointmentReminderRepository, AppointmentReminderRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            // Add services to the container.
            builder.Services.AddControllers();

            //Add DbContext SQL
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            //Add DBContext PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("PetHubDbConnection")));


            //Add Identity services, chỗ này là đăng ký để ASP.Net tự DI dùm ở chỗ AuthService
            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();

            //Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            //Config JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>{
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });


            //ADD CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()   // Cho phép tất cả các nguồn (domain)
                          .AllowAnyHeader()   // Cho phép tất cả các Header
                          .AllowAnyMethod();  // Cho phép tất cả các phương thức (GET, POST, PUT, DELETE...)
                });
            });

            //Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            //seed data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();

                    // 1. Tự động chạy Migration (Tạo bảng trên Neon nếu chưa có)
                    await context.Database.MigrateAsync();

                    // 2. Chạy SeedData
                    var seedData = new SeedData();
                    await seedData.InitializeAsync(services);

                    Console.WriteLine("Database Migration & Seed completed successfully!");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                }
            }


            // if (app.Environment.IsDevelopment())
            // {
            //     app.UseSwagger();
            //     app.UseSwaggerUI();
            // }
            app.UseSwagger();
            app.UseSwaggerUI();

            //AllowAll
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });

            //environment variable for port, default to 8080 if not set
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Run($"http://0.0.0.0:{port}");

            //chạy test local thì dùng cái này cho nhanh, chạy trên server thì dùng cái trên
            //app.Run();
        }
    }
}
