using NoSQL_Project.Enums;
using NoSQL_Project.Models;

namespace NoSQL_Project.Repositories.Interfaces
{
	public interface IEmployeeRepository
	{
		Task<List<Employee>> GetAllAsync(Location? location, UserRole? userRole);
		Task<Employee> GetByIdAsync(string id);
		Task AddEmployeeAsync(Employee employees);
		Task UpdateEmployeeAsync(Employee employees);
		Task<bool> SoftDeleteAsync(string id);
		Task<Employee?> GetByLoginCredentialAsync(string email, string password);
		Task<bool> EmailAddressExistsAsync(string email);
	}
}
