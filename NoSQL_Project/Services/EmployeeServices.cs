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
		public async Task<List<Employees>> GellAsync()
		{
			return await _employeeRepo.GellAsync();
		}

		public async Task<Employees> GetByIdAsync(string id)
		{
			return await _employeeRepo.GetByIdAsync(id);
		}

		public async Task AddEmployeeAsync(Employees employees)
		{			
			await _employeeRepo.AddEmployeeAsync(employees);
		}

		public async Task UpdateEmployeeAsync(Employees employees) 
		{
			await _employeeRepo.UpdateEmployeeAsync(employees);
		}
		public async Task<bool> SoftDeleteAsync(string id)
		{
			return await _employeeRepo.SoftDeleteAsync(id);
		}
	}
}

