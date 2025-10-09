using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NoSQL_Project.Enums;
using MongoDB.Bson.Serialization.IdGenerators;

namespace NoSQL_Project.Models
{
	public class Employees
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? EmployeeId { get; set; }
		public string FirstName { get; set; } = "";
		public string LastName { get; set; } = "";
		public Gender Gender { get; set; } = Gender.other;
		public UserRole UserRole { get; set; } = UserRole.employee;
		public bool IsActive { get; set; } = true;
		public string Email { get; set; } = "";
		public  string PhoneNumber { get; set; } = "";
		public Location Location { get; set; } = Location.Haarlem;
	}
}
