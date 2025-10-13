using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;


namespace NoSQL_Project.Repositories
{

	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly IMongoCollection<Employee> _employees;

		public EmployeeRepository(IMongoDatabase db)
		{
			_employees = db.GetCollection<Employee>("Employees");
		}

		public async Task<List<Employee>> GellAsync()
		{
			return await _employees
				.Find(s => true)
				.SortByDescending(e => e.IsActive)  // Active first (true comes before false)
				.ThenBy(e => e.FirstName)           // Then alphabetically by FirstName
				.ToListAsync();	
		}
		public async Task<Employee> GetByIdAsync(string id)
		{
			return await _employees.Find(s => s.EmployeeId == id).FirstOrDefaultAsync();
		}
		public async Task AddEmployeeAsync(Employee employees)
		{
			await _employees.InsertOneAsync(employees);
		}
		public async Task UpdateEmployeeAsync(Employee employees)
		{
			await _employees.ReplaceOneAsync(s => s.EmployeeId == employees.EmployeeId, employees);
		}
		public async Task<bool> SoftDeleteAsync(string id)
		{
			var employee = await GetByIdAsync(id);
			if (employee == null || !employee.IsActive)
				return false; // Not found or already inactive

			var filter = Builders<Employee>.Filter.Eq(e => e.EmployeeId, id);
			var update = Builders<Employee>.Update.Set(e => e.IsActive, false);

			var result = await _employees.UpdateOneAsync(filter, update);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}
		public async Task<Employee?> GetByLoginCredentialAsync(string email, string password)
		{
			var filter = Builders<Employee>.Filter.Eq(e => e.Email, email) &
						 Builders<Employee>.Filter.Eq(e => e.Password, password);

			return await _employees.Find(filter).FirstOrDefaultAsync();
		}
		public async Task<bool> EmailAddressExistsAsync(string email)
		{
			var filter = Builders<Employee>.Filter.Eq(e => e.Email, email);
			var count = await _employees.CountDocumentsAsync(filter);
			return count > 0;
		}
	}
}

