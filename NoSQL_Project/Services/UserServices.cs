using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories;
using NoSQL_Project.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace NoSQL_Project.Services
{
	public class UserServices : IUserServices
	{
		private readonly IUserRepository _userRepository;
		public UserServices(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}
		public List<User> GetAll()
		{
			return _userRepository.GetAll();
		}
		public void Add(User user)
		{
			// Hash the password before storing it
			user.Password = HashPassword(user.Password);
			_userRepository.Add(user);
		}
		private string HashPassword(string password)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				return Convert.ToBase64String(hashBytes);
			}
		}
		// Future:
		public User? GetById(string id)
		{
			return _userRepository.GetById(id);
		}
		public void Update(User user)
		{
			_userRepository.Update(user);
		}
		public void Delete(string id)
		{
			_userRepository.Delete(id);
		}
		public bool IsEmailUnique(string email)
		{
			return _userRepository.IsEmailUnique(email);
		}
		public User? GetByLoginCredentials(string email, string password) 
		{
			return _userRepository.GetByLoginCredentials(email, HashPassword(password));
		}
	}
}
