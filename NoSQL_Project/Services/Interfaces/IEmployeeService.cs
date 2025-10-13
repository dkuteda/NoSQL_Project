using NoSQL_Project.Models;
using NoSQL_Project.Enums;	

namespace NoSQL_Project.Services
{
	public interface IEmployeeService
	{
		Task<List<Employee>> GellAsync();
		Task<Employee> GetByIdAsync(string id);
		Task AddEmployeeAsync(Employee employees);
		Task UpdateEmployeeAsync(Employee employees);
		Task<bool> SoftDeleteAsync(string id);
		Task<Employee?> GetByLoginCredentialAsync(string firstName, string password);
	}
}
