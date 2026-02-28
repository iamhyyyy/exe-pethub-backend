using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using EXE_PET_HUB.Infrastructure.Data;
using EXE_PET_HUB.Infrastructure.Identity;
using EXE_PET_HUB.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IPetRepository, PetRepository>();
            builder.Services.AddScoped<PetService>();

            // Add services to the container.
            builder.Services.AddControllers();

            //Add DbContext SQL
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            //Add DBContext PostgreSQL
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            //debug sql
            var conn = builder.Configuration.GetConnectionString("DefaultConnection");

            // Fallback: nếu không lấy được từ appsettings, thử đọc từ biến môi trường (deploy như Render)
            if (string.IsNullOrEmpty(conn))
            {
                conn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? Environment.GetEnvironmentVariable("DATABASE_URL");
            }

            // Một số platform (hoặc UI env var) có thể tự thêm dấu " ở đầu/cuối connection string
            // => Trim whitespace + dấu quote để Npgsql parse không bị lỗi "Format of the initialization string..."
            if (!string.IsNullOrEmpty(conn))
            {
                conn = conn.Trim();
                if ((conn.StartsWith("\"") && conn.EndsWith("\"")) ||
                    (conn.StartsWith("'") && conn.EndsWith("'")))
                {
                    conn = conn.Substring(1, conn.Length - 2);
                }
            }

            Console.WriteLine("==== CONNECTION STRING FROM RENDER ====");
            Console.WriteLine(string.IsNullOrEmpty(conn) ? "<NULL OR EMPTY>" : conn);
            Console.WriteLine("=======================================");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(conn)
            );

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
