using CityInfo.io.DbContexts;
using CityInfo.io.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CityInfo.io
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/cityInfo.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);
            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole();

            // Add services to the container.

            

            builder.Services.AddControllers(
                options => options.ReturnHttpNotAcceptable = true
                ).AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters() ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            #if DEBUG
            builder.Services.AddTransient<IMailService,LocalMailService>();
            #else
            builder.Services.AddTransient<IMailService,CloudMailService>();
            #endif
            builder.Services.AddSingleton<CitiesDataStore>();
            builder.Host.UseSerilog();
            builder.Services.AddDbContext<CityInfoContext>(dbContextOptions => dbContextOptions.UseSqlite(
                builder.Configuration["ConnectionStrings:CitylnfoDBConnectionString"]));
            
            builder.Services.AddScoped<ICityInfoRepository,CityInfoRepository>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });

            var app = builder.Build();

           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            

            app.Run();
        }
    }
}