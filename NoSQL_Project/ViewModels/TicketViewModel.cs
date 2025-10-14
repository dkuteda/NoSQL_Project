using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class TicketViewModel
    {
        //used for CUD of a single ticket
        public Ticket Ticket { get; set; }

        public int TotalTickets { get; set; }= 0;

        // Emuns
        public IEnumerable<SelectListItem> StatusOptions { get; set; }
        public IEnumerable<SelectListItem> TypeOfIncidentOptions { get; set; }
        public IEnumerable<SelectListItem> PriorityOptions { get; set; }

        // Used for index
        public List<Ticket> TicketList { get; set; }
    }
}
