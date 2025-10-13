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

		public async Task<List<Employee>> GellAsync()
		{
			return await _employeeRepo.GellAsync();
		}

		public async Task<Employee> GetByIdAsync(string id)
		{
			return await _employeeRepo.GetByIdAsync(id);
		}

		public async Task AddEmployeeAsync(Employee employees)
        {
            employees.Password = HashPassword(employees.Password);
            await _employeeRepo.AddEmployeeAsync(employees);
		}

		public async Task UpdateEmployeeAsync(Employee employees) 
		{
			await _employeeRepo.UpdateEmployeeAsync(employees);
		}
		public async Task<bool> SoftDeleteAsync(string id)
		{
			return await _employeeRepo.SoftDeleteAsync(id);
		}
		public async Task<Employee?> GetByLoginCredentialAsync(string firstName, string password)
		{
			// hash user-entered password before checking
			var hashed = HashPassword(password);
			return await _employeeRepo.GetByLoginCredentialAsync(firstName, hashed);
		}
		private static string HashPassword(string password)
		{
			using var sha256 = SHA256.Create();
			var bytes = Encoding.UTF8.GetBytes(password);
			var hash = sha256.ComputeHash(bytes);
			return Convert.ToBase64String(hash);
		}
	}
}

