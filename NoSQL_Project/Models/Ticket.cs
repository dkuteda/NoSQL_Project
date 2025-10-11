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
		public string? TicketId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; } = "";
		public TicketStatus Status { get; set; } = TicketStatus.open;
        public Priority Priority { get; set; } = Priority.normal;
		public DateTime Deadline { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
		public TypeOfIncident TypeOfIncident { get; set; } = TypeOfIncident.software;
		public List<ResolutionStep> ResolutionSteps { get; set; }
	}
}
