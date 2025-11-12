using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
	public class UserTokens
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? TokenId { get; set; }
		public string EmployeeId { get; set; } = null!;
		public string LoginProvider { get; set; } = null!;
		public string Name { get; set; } = null!;
		public string Value { get; set; } = null!;
	}
}
