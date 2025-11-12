using MadEyeMatt.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services;
using NoSQL_Project.Services.Interfaces;

namespace NoSQL_Project
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Load .env before building configuration so env vars are available
			DotNetEnv.Env.TraversePath().Load();

			var builder = WebApplication.CreateBuilder(args);

			// Register MongoClient as a SINGLETON (thread-safe, pooled)
			builder.Services.AddSingleton<IMongoClient>(sp =>
			{
				var conn = builder.Configuration["Mongo:ConnectionString"];
				if (string.IsNullOrWhiteSpace(conn))
					throw new InvalidOperationException("Mongo:ConnectionString is not configured. Did you set it in .env?");

				var settings = MongoClientSettings.FromConnectionString(conn);
				return new MongoClient(settings);
			});

			// Register IMongoDatabase as SCOPED (per request)
			builder.Services.AddScoped(sp =>
			{
				var client = sp.GetRequiredService<IMongoClient>();
				var dbName = builder.Configuration["Mongo:Database"];
				if (string.IsNullOrWhiteSpace(dbName))
					throw new InvalidOperationException("Mongo:Database is not configured in appsettings.json.");

				return client.GetDatabase(dbName);
			});

			// Register repositories and services
			builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
			builder.Services.AddScoped<IEmployeeService, EmployeeService>();
			builder.Services.AddScoped<ITicketRepository, TicketRepository>();
			builder.Services.AddScoped<ITicketService, TicketService>();
			builder.Services.AddScoped<IEmailService, EmailService>();

			builder.Services.AddControllersWithViews();

			// Register ASP.NET Core Identity with MongoDB stores
			builder.Services.AddIdentity<Employee, MongoIdentityRole>()
				.AddMongoDbStores<Employee, MongoIdentityRole, Guid>(
					builder.Configuration["Mongo:ConnectionString"],
					builder.Configuration["Mongo:Database"])
				.AddDefaultTokenProviders();

			// Optional: configure Identity options
			builder.Services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 6;
				options.User.RequireUniqueEmail = true;
			});

			// Session
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			var app = builder.Build();

			// Middleware pipeline
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseSession();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Employees}/{action=Login}/{id?}");

			app.Run();
		}
	}
}
