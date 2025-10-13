using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class TicketViewModel
    {
        public string TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public Employee CreatedBy { get; set; }
        public string HandledBy { get; set; }
        public TypeOfIncident TypeOfIncident { get; set; }
        public List<ResolutionStepViewModel>ResolutionSteps { get; set; }

        // Emuns
        public IEnumerable<SelectListItem> TypeOfIncidentOptions { get; set; }
        public IEnumerable<SelectListItem> PriorityOptions { get; set; }

        // Used for index
        public List<Ticket> TicketList { get; set; }
    }
}
