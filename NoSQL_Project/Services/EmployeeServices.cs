using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories;
using NoSQL_Project.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;


namespace NoSQL_Project.Services
{
	public class EmployeeServices : IEmployeeServices
	{
		private readonly IEmployeeRepository _employeeRepo;
		public EmployeeServices(IEmployeeRepository employeeRepository)
		{
			_employeeRepo = employeeRepository;
		}
		
	
		private string HashPassword(string password)
		{

			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				return Convert.ToBase64String(hashBytes);
			}
		}
		public async Task Createasync(Employees employee)
		{
			await _employeeRepo.AddEmployeeAsync(employee);
		}
		public async Task<List<Employees>> GellAsync()
		{
			return await _employeeRepo.GellAsync();
		}

		public async Task<Employees> GetByIdAsync(string id)
		{
			return await _employeeRepo.GetByIdAsync(id);
		}

		//public async Task Createasync(Employees employee)
		//{
		//	employee.PasswordHash = HashPassword(employee.PasswordHash);
		//	await _employeeRepo.Createasync(employee);
		//}

		public async Task Updateasync(string id , Employees employee)
		{
			await _employeeRepo.Updateasync(id , employee);
		}

		public async Task Deleteasync(string id)
		{
			await _employeeRepo.Deleteasync(id);
		}
		//public async Task<Employees?> GetByLoginCredentials(string email, string password)
		//{
		//   var hashedPassword = HashPassword(password);
		//   return await _employeeRepo.GetByLoginCredentials(email, hashedPassword);

		//}
	}
}

