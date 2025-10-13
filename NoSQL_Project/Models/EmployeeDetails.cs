using MongoDB.Bson.Serialization.Attributes;

namespace NoSQL_Project.Models
{
    public class EmployeeDetails
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNr { get; set; }
    }
}
