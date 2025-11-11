using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoSQL_Project.Models
{
    [BsonIgnoreExtraElements]
    public class ResolutionStep
    {
        [BsonId]
        [BsonElement("Id")]  
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("PresentHandler")]
        public EmployeeDetails PresentHandler { get; set; }
        public string Action { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ResolutionStep(EmployeeDetails presentHandler, string action)
        {
            PresentHandler = presentHandler;
            Action = action;
        }

        public ResolutionStep() { }


    }
}
