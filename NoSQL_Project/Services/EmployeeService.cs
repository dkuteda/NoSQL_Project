using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MongoDB.Driver;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;
using NoSQL_Project.Repositories;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;


namespace NoSQL_Project.Services
{
	public class EmployeeService : IEmployeeService
	{
		private readonly IEmployeeRepository _employeeRepo;
		private readonly UserManager<Employee> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IEmailService _emailService;


		public EmployeeService(IEmployeeRepository employeeRepository, UserManager<Employee> userManager, IConfiguration configuration,
			IEmailService emailService)
		{
			_employeeRepo = employeeRepository;
			_userManager = userManager;
			_configuration = configuration;
			_emailService = emailService;
		}

		public async Task<List<Employee>> GetAllAsync(Location? location, UserRole? userRole) 
		{
			return await _employeeRepo.GetAllAsync(location, userRole);
		}

		public async Task<Employee> GetByIdAsync(string id)
		{
			return await _employeeRepo.GetByIdAsync(id);
		}

		public async Task AddEmployeeAsync(Employee employees)
        {
			if (EmailAddressExistsAsync(employees.Email).Result)
				throw new Exception("Email address already in use.");

			employees.Password = HashPassword(employees.Password);
            await _employeeRepo.AddEmployeeAsync(employees);
		}

		public async Task UpdateEmployeeAsync(string id, Employee employee)
		{
			if (!string.IsNullOrWhiteSpace(employee.Password)) 
			{
				employee.Password = HashPassword(employee.Password);
				await _employeeRepo.UpdateEmployeeAsync(id, employee);
			}
			else 
			{
				var existingEmployee = await _employeeRepo.GetByIdAsync(id);
				if (existingEmployee != null)
				{
					employee.Password = existingEmployee.Password;
					await _employeeRepo.UpdateEmployeeAsync(id, employee);
				}
			}
		}
		public async Task<bool> SoftDeleteAsync(string id)
		{
			return await _employeeRepo.SoftDeleteAsync(id);
		}

		public async Task<bool> EmailAddressExistsAsync(string email)
		{
			return await _employeeRepo.EmailAddressExistsAsync(email);
		}

		public async Task<bool> SendPasswordResetLinkAsync(string email) 
		{
			var user = await _employeeRepo.GetByEmailAsync(email);

			if (user == null)
				return false;

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

			var baseUrl = _configuration["AppSettings:BaseUrl"];
			var resetLink = $"{baseUrl}/Account/ResetPassword?email={user.Email}&token={encodedToken}";
			
			await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, resetLink);

			return true;
		}

		public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
		{
			var user = await _employeeRepo.GetByEmailAsync(model.Email);

			if (user == null)
				return IdentityResult.Failed(new IdentityError { Description = "Invalid request." });

			var decodedBytes = WebEncoders.Base64UrlDecode(model.Token);
			var decodedToken = Encoding.UTF8.GetString(decodedBytes);

			var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);

			if (result.Succeeded)
				await _userManager.UpdateSecurityStampAsync(user);

			return result;
		}

		//Hamza' Code
		public async Task<Employee?> GetByLoginCredentialAsync(string email, string password)
		{
			// hash entered password before checking
			var hashed = HashPassword(password);
			return await _employeeRepo.GetByLoginCredentialAsync(email, hashed);
		}
		private static string HashPassword(string password)
		{
			// use B authentication
			using var sha256 = SHA256.Create();
			var bytes = Encoding.UTF8.GetBytes(password);
			var hash = sha256.ComputeHash(bytes);
			return Convert.ToBase64String(hash);
		}	
		
		public async Task<List<Employee>> AutocompleteSearchEmployees(string name)
		{
			return await _employeeRepo.AutocompleteSearchEmployees(name);
        }
    }
}

