using NoSQL_Project.Models;
using NoSQL_Project.Enums;	

namespace NoSQL_Project.Services
{
	public interface IEmployeeServices
	{
		Task<List<Employees>> GellAsync();
		Task<Employees> GetByIdAsync(string id);
		Task AddEmployeeAsync(Employees employees);
		Task UpdateEmployeeAsync(Employees employees);
	}
}
