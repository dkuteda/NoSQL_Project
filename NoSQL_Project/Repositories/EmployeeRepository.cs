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
			_employees = db.GetCollection<Employees>("employees");
		}

		public async Task<List<Employees>> GellAsync()
		{
			return await _employees.Find(s => true).ToListAsync();
		}
		public async Task<Employees> GetByIdAsync(string id)
		{
			return await _employees.Find(s => s.Id == id).FirstOrDefaultAsync();
		}
		public async Task Createasync(Employees employees)
		{
			await _employees.InsertOneAsync(employees);
		}
		public async Task Updateasync(string id, Employees employees)
		{
			await _employees.ReplaceOneAsync(e => e.Id == id, employees);
		}
		public async Task Deleteasync(string id)
		{
			await _employees.DeleteOneAsync(s => s.Id == id);
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

