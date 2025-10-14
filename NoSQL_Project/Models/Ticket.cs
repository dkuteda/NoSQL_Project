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
		public required string TicketId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; } = "";

        [BsonRepresentation(BsonType.String)]
        public TicketStatus Status { get; set; } = TicketStatus.open;

        [BsonRepresentation(BsonType.String)]
        public Priority Priority { get; set; } = Priority.normal;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime Deadline { get; set; }

        [BsonElement("CreatedBy")]
        public EmployeeDetails CreatedBy { get; set; }

        [BsonElement("HandledBy")]
        public EmployeeDetails HandledBy { get; set; } = new EmployeeDetails();

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [BsonRepresentation(BsonType.String)]
        public TypeOfIncident TypeOfIncident { get; set; } = TypeOfIncident.software;

        [BsonElement("ResolutionSteps")]
        public List<ResolutionStep> ResolutionSteps { get; set; }

        public Ticket () {}

	}
}
