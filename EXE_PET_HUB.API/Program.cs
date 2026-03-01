using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using EXE_PET_HUB.Infrastructure.Data;
using EXE_PET_HUB.Infrastructure.Identity;
using EXE_PET_HUB.Infrastructure.Repositories;
using EXE_PET_HUB.Infrastructure.Services;
using EXE_PET_HUB.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure email settings
            builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<PetService>();
            builder.Services.AddScoped<IPetRepository, PetRepository>();

            // Add services to the container.
            builder.Services.AddControllers();

            //Add DbContext SQL
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            //Add DBContext PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            //Add Identity services
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();

            //Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();


            // if (app.Environment.IsDevelopment())
            // {
            //     app.UseSwagger();
            //     app.UseSwaggerUI();
            // }
            app.UseSwagger();
            app.UseSwaggerUI();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            //environment variable for port, default to 8080 if not set
             var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Run($"http://0.0.0.0:{port}");

            //chạy test local thì dùng cái này cho nhanh, chạy trên server thì dùng cái trên
            //app.Run();
        }
    }
}
