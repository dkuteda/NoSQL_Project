using MongoDB.Driver;
using NoSQL_Project.Enums;
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

		public async Task<List<Employee>> GetAllAsync(Location? location, UserRole? userRole)
		{
			// remove gender
			var filterBuilder = Builders<Employee>.Filter;
			var filter = filterBuilder.Empty;

			if (location.HasValue)
				filter &= filterBuilder.Eq(e => e.Location, location.Value);

			if (userRole.HasValue)
				filter &= filterBuilder.Eq(e => e.UserRole, userRole.Value);

			return await _employees.Find(filter).ToListAsync();
		}

		public async Task<Employee> GetByIdAsync(string id)
		{
			return await _employees.Find(s => s.EmployeeId == id).FirstOrDefaultAsync();
		}		

		public async Task UpdateEmployeeAsync(string id, Employee employee)
		{
			var filter = Builders<Employee>.Filter.Eq(e => e.EmployeeId, id); ;

			var update = Builders<Employee>.Update
				.Set(s => s.FirstName, employee.FirstName)
				.Set(s => s.LastName, employee.LastName)
				.Set(s => s.Password, employee.Password)
				.Set(s => s.Email, employee.Email)
				.Set(s => s.PhoneNumber, employee.PhoneNumber)
				.Set(s => s.Location, employee.Location)
				.Set(s => s.UserRole, employee.UserRole);

			await _employees.UpdateOneAsync(filter, update);
		}

		public async Task AddEmployeeAsync(Employee employees)
		{
			await _employees.InsertOneAsync(employees);
		}

		public async Task<bool> SoftDeleteAsync(string id)
		{
			// use update and change in service layer
			var employee = await GetByIdAsync(id);
			if (employee == null || !employee.IsActive)
				return false; // Not found or already inactive

			var filter = Builders<Employee>.Filter.Eq(e => e.EmployeeId, id);
			var update = Builders<Employee>.Update.Set(e => e.IsActive, false);

			var result = await _employees.UpdateOneAsync(filter, update);
			return result.IsAcknowledged && result.ModifiedCount > 0;
		}

		public async Task<bool> EmailAddressExistsAsync(string email)
		{
			var filter = Builders<Employee>.Filter.Eq(e => e.Email, email);
			var count = await _employees.CountDocumentsAsync(filter);
			return count > 0;
		}


		//Hamzas code for getting the login info 
		public async Task<Employee?> GetByLoginCredentialAsync(string email, string hashedPassword)
		{
			var filter = Builders<Employee>.Filter.Eq(e => e.Email, email) &
						 Builders<Employee>.Filter.Eq(e => e.Password, hashedPassword);

			return await _employees.Find(filter).FirstOrDefaultAsync();
		}		
	}
}

