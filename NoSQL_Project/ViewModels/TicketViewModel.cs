using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class TicketViewModel
    {
        public Ticket ticket { get; set; }

        // Emuns
        public IEnumerable<SelectListItem> TypeOfIncidentOptions { get; set; }
        public IEnumerable<SelectListItem> PriorityOptions { get; set; }

        // Used for index
        public List<Ticket> ticketList { get; set; }
    }
}
