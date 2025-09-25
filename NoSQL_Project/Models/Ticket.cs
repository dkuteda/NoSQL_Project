using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NoSQL_Project.Enums;
using MongoDB.Bson.Serialization.IdGenerators;
using System.Globalization;

namespace NoSQL_Project.Models
{
	public class Ticket
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }
		public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
		public TypeOfIncident TypeOfIncident { get; set; } = TypeOfIncident.software;
		public Priority Priority { get; set; } = Priority.normal;
		public string Description { get; set; } = "";
	}
}
