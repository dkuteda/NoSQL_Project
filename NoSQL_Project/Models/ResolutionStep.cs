using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoSQL_Project.Models
{
    public class ResolutionStep
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int ResolutionStepNr { get; set; }= 0;

        [BsonElement("PresentHandler")]
        public EmployeeDetails PresentHandler { get; set; }
        public string Action { get; set; }

    }
}
