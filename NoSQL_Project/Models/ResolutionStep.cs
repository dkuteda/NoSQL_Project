using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NoSQL_Project.Models
{
    public class ResolutionStep
    {
        public ResolutionStep(string? id, EmployeeDetails presentHandler, string action)
        {
            Id = id;
            PresentHandler = presentHandler;
            Action = action;
        }
        public ResolutionStep() { }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int ResolutionStepNr { get; set; }= 0;

        [BsonElement("PresentHandler")]
        public EmployeeDetails PresentHandler { get; set; }
        public string Action { get; set; }


    }
}
