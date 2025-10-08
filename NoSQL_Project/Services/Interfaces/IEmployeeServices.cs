using NoSQL_Project.Models;
using NoSQL_Project.Enums;	

namespace NoSQL_Project.Services
{
	public interface IEmployeeServices
	{
		Task<List<Employees>> GellAsync();
		Task<Employees> GetByIdAsync(string id);
		Task Createasync(Employees employees);
		Task Deleteasync(string id);
		Task Updateasync(string id , Employees employees);
		//Employees? GetByLoginCredentials(string email, string password);
	}
}
