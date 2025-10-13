using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class TicketViewModel
    {
        // Emuns
        public IEnumerable<SelectListItem> StatusOptions { get; set; }
        public IEnumerable<SelectListItem> TypeOfIncidentOptions { get; set; }
        public IEnumerable<SelectListItem> PriorityOptions { get; set; }

        // Used for index
        public List<Ticket> TicketList { get; set; }
    }
}
