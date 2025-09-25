using NoSQL_Project.Models;
using NoSQL_Project.Enums;	

namespace NoSQL_Project.Services
{
	public interface IUserServices
	{
		List<User> GetAll();
		void Add(User user);

		// Future:
		User? GetById(string id);
		void Update(User user);
		void Delete(string id);
		bool IsEmailUnique(string email);
		public User? GetByLoginCredentials(string email, string password);
	}
}
