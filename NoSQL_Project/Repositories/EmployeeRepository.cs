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
			return await _employees
				.Find(s => true)
				.SortByDescending(e => e.IsActive)  // Active first (true comes before false)
				.ThenBy(e => e.FirstName)           // Then alphabetically by FirstName
				.ToListAsync();	
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
		public async Task<bool> SoftDeleteAsync(string id)
		{
			var employee = await GetByIdAsync(id);
			if (employee == null || !employee.IsActive)
				return false; // Not found or already inactive

			var filter = Builders<Employees>.Filter.Eq(e => e.EmployeeId, id);
			var update = Builders<Employees>.Update.Set(e => e.IsActive, false);

			var result = await _employees.UpdateOneAsync(filter, update);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}
		public async Task<Employees?> GetByLoginCredentialAsync(string firstName, string password)
		{
			var filter = Builders<Employees>.Filter.Eq(e => e.FirstName, firstName) &
						 Builders<Employees>.Filter.Eq(e => e.Password, password);

			return await _employees.Find(filter).FirstOrDefaultAsync();
		}

	}
}

