using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using NoSQL_Project.Enums;

namespace NoSQL_Project.Models
{
    public class ResolutionStep
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string? Id { get; set; }
        public int ResolutionStepNr { get; set; }
        public Employees PresentHandler { get; set; }
        public string Action { get; set; }
    }
}
