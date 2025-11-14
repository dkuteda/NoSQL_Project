using MongoDB.Bson;
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
			var filterBuilder = Builders<Employee>.Filter;

			var filter = filterBuilder.Eq(e => e.IsActive, true);

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

        public async Task<Employee> GetByEmailAsync(string email)
        {
            return await _employees.Find(s => s.Email == email).FirstOrDefaultAsync();
		}

		public async Task AddEmployeeAsync(Employee employees)
		{
			await _employees.InsertOneAsync(employees);
		}
		public async Task UpdateEmployeeAsync(string id, Employee employee)
		{
			var filter = Builders<Employee>.Filter.Eq(e => e.EmployeeId, id);

			var updates = new List<UpdateDefinition<Employee>>();

			// Strings
			if (!string.IsNullOrEmpty(employee.FirstName))
				updates.Add(Builders<Employee>.Update.Set(e => e.FirstName, employee.FirstName));

			if (!string.IsNullOrEmpty(employee.LastName))
				updates.Add(Builders<Employee>.Update.Set(e => e.LastName, employee.LastName));

			if (!string.IsNullOrEmpty(employee.Password))
				updates.Add(Builders<Employee>.Update.Set(e => e.Password, employee.Password));

			if (!string.IsNullOrEmpty(employee.Email))
				updates.Add(Builders<Employee>.Update.Set(e => e.Email, employee.Email));

			if (!string.IsNullOrEmpty(employee.PhoneNumber))
				updates.Add(Builders<Employee>.Update.Set(e => e.PhoneNumber, employee.PhoneNumber));

			// Nullable Enums — only update if HasValue
			if (employee.Location.HasValue)
				updates.Add(Builders<Employee>.Update.Set(e => e.Location, employee.Location.Value));

			if (employee.UserRole.HasValue)
				updates.Add(Builders<Employee>.Update.Set(e => e.UserRole, employee.UserRole.Value));

			// Boolean — always update (since bool always has a value)
			updates.Add(Builders<Employee>.Update.Set(e => e.IsActive, employee.IsActive));

			if (updates.Count > 0)
			{
				var update = Builders<Employee>.Update.Combine(updates);
				await _employees.UpdateOneAsync(filter, update);
			}
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

        //Nana Yaa's code for searching for employees using Atlas Search Autocomplete
        //well, gotta use a pipeline somehow
        public async Task<List<Employee>> AutocompleteSearchEmployees(string name)
        {
            var pipeline = new[]
            {
                new BsonDocument("$search", new BsonDocument
                {
                    { "index", "employeeSearch" },
                    { "compound", new BsonDocument
                        {
                            { "should", new BsonArray
                                {
                                    new BsonDocument("autocomplete", new BsonDocument
                                    {
                                        { "query", name },
                                        { "path", "FirstName" }
                                    }),
                                    new BsonDocument("autocomplete", new BsonDocument
                                    {
                                        { "query", name },
                                        { "path", "LastName" }
                                    })
                                }
                            }
                        }
                    }
                }),
                new BsonDocument("$limit", 10)
            };
            var results = await _employees.Aggregate<Employee>(pipeline).ToListAsync();
            return results;
        }


    }
}

