using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories;
using NoSQL_Project.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;


namespace NoSQL_Project.Services
{
	public class EmployeeService : IEmployeeService
	{
		private readonly IEmployeeRepository _employeeRepo;

		public EmployeeService(IEmployeeRepository employeeRepository)
		{
			_employeeRepo = employeeRepository;
		}

		public async Task<List<Employee>> GetAllAsync(Gender? gender, Location? location, UserRole? userRole) 
		{
			return await _employeeRepo.GetAllAsync(gender, location, userRole);
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

		public async Task UpdateEmployeeAsync(Employee employees) 
		{
			if (EmailAddressExistsAsync(employees.Email).Result)
				throw new Exception("Email address already in use");

			await _employeeRepo.UpdateEmployeeAsync(employees);
		}
		public async Task<bool> SoftDeleteAsync(string id)
		{
			return await _employeeRepo.SoftDeleteAsync(id);
		}
		public async Task<Employee?> GetByLoginCredentialAsync(string email, string password)
		{
			// hash user-entered password before checking
			var hashed = HashPassword(password);
			return await _employeeRepo.GetByLoginCredentialAsync(email, hashed);
		}
		private static string HashPassword(string password)
		{
			using var sha256 = SHA256.Create();
			var bytes = Encoding.UTF8.GetBytes(password);
			var hash = sha256.ComputeHash(bytes);
			return Convert.ToBase64String(hash);
		}
		public async Task<bool> EmailAddressExistsAsync(string email)
		{
			return await _employeeRepo.EmailAddressExistsAsync(email);
		}
	}
}

