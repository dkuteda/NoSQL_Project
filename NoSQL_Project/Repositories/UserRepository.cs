using MongoDB.Driver;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Repositories
{

	public class UserRepository : IUserRepository
	{
		private readonly IMongoCollection<User> _users;

		public UserRepository(IMongoDatabase db)
		{
			_users = db.GetCollection<User>("users");
		}

		public List<User> GetAll() =>
			_users.Find(_ => true).ToList();

		public void Add(User user) =>
			_users.InsertOne(user);

		public User? GetById(string id) =>
			_users.Find(u => u.Id == id).FirstOrDefault();

		public void Update(User user) =>
			_users.ReplaceOne(u => u.Id == user.Id, user);

		public void Delete(string id) =>
			_users.DeleteOne(u => u.Id == id);
	}
}
