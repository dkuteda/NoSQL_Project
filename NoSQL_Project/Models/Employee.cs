using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NoSQL_Project.Enums;

namespace NoSQL_Project.Models
{
	public class Employee
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? EmployeeId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }

		public string? Password { get; set; }

		[BsonRepresentation(BsonType.String)]
		public Gender? Gender { get; set; }

		[BsonRepresentation(BsonType.String)]
		public UserRole? UserRole { get; set; }
		public bool IsActive { get; set; } = true;
		public string? Email { get; set; }
		public  string? PhoneNumber { get; set; }

		[BsonRepresentation(BsonType.String)]
		public Location? Location { get; set; }
	}
}
