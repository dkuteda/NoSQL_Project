using NoSQL_Project.Models;

namespace NoSQL_Project.Repositories.Interfaces
{
	public interface IEmployeeRepository
	{
		Task<List<Employees>> GellAsync();
		Task<Employees> GetByIdAsync(string id);
		Task AddEmployeeAsync(Employees employees);
	}
}
