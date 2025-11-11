using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    public class EmployeeDetails
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string EmployeeId { get; set; } = "";
        [BsonElement("Firstname")]
        public string Firstname { get; set; } = "";
        [BsonElement("Lastname")]
        public string Lastname { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNr { get; set; } = "";
    }
}
