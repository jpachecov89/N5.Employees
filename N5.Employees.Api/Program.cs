using Microsoft.EntityFrameworkCore;
using N5.Employees.Api.Middlewares;
using N5.Employees.Application.Interfaces.Kafka;
using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Application.Permissions.Commands.CreatePermission;
using N5.Employees.Infrastructure.ElasticSearch.Configurations;
using N5.Employees.Infrastructure.Kafka;
using N5.Employees.Infrastructure.Kafka.Configurations;
using N5.Employees.Infrastructure.Persistence;
using N5.Employees.Infrastructure.Persistence.Contexts;
using N5.Employees.Infrastructure.Persistence.Repositories;
using Serilog;

namespace N5.Employees.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();
            builder.Services.AddScoped<IPermissionRepository,  PermissionRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreatePermissionCommand).Assembly);
            });

            builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
            builder.Services.AddSingleton<IKafkaProducerSerivce, KafkaProducerService>();

            builder.Services.AddHostedService<KafkaConsumerService>();

            builder.Services.AddElasticSearchService(builder.Configuration);

			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

				using var scope = app.Services.CreateScope();
				var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
				dbContext.Database.Migrate();
			}

            app.UseMiddleware<ExceptionHandlerMiddleware>();
			app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}