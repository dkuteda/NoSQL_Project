using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;


namespace NoSQL_Project.Repositories
{

	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly IMongoCollection<Employees> _employees;

		public EmployeeRepository(IMongoDatabase db)
		{
			_employees = db.GetCollection<Employees>("Employees");
		}

		public async Task<List<Employees>> GellAsync()
		{
			return await _employees.Find(s => true).ToListAsync();
		}
		public async Task<Employees> GetByIdAsync(string id)
		{
			return await _employees.Find(s => s.EmployeeId == id).FirstOrDefaultAsync();
		}
		public async Task AddEmployeeAsync(Employees employees)
		{
			await _employees.InsertOneAsync(employees);
		}
		public async Task UpdateEmployeeAsync(Employees employees)
		{
			await _employees.ReplaceOneAsync(s => s.EmployeeId == employees.EmployeeId, employees);
		}
		public async Task Deleteasync(string id)
		{
			await _employees.DeleteOneAsync(s => s.EmployeeId == id);
		}
		//public async Task<Employees?> GetByLoginCredentialAsync(string email, string password)
		//{
		//	// Hash the incoming password for comparison
		//	var hashedPassword = HashPassword(password);

		//	var filter = Builders<Employees>.Filter.Eq(e => e.Email, email) &
		//				 Builders<Employees>.Filter.Eq(e => e.PasswordHash, hashedPassword);

		//	return await _employees.Find(filter).FirstOrDefaultAsync();
		//}
	}
}

