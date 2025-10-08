using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NoSQL_Project.Enums;
using MongoDB.Bson.Serialization.IdGenerators;

namespace NoSQL_Project.Models
{
	public class Employees
	{
		// This will be the primary key in MongoDB.
		// [BsonId] tells MongoDB this field is the "_id".
		// [BsonRepresentation(BsonType.ObjectId)] lets us use string in C#,
		// while MongoDB still stores it as a real ObjectId internally.
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		// A simple string field for the user's name.
		// Default = "" so it's never null when creating a new User.
		public string FirstName { get; set; } = "";
		public string LastName { get; set; } = "";
		public string Name { get { return FirstName + " " + LastName; } }
		
		public UserRole UserRole { get; set; } = UserRole.employee;

		// A simple string field for the user's email.
		// Later we could add validation (e.g. DataAnnotations).
		public string Email { get; set; } = "";
		public  string PhoneNumber { get; set; } = "";
		public Location Location { get; set; } = Location.Haarlem;
	}
}
