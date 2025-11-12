using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class TicketViewModel
    {
        //used for CRUD of a single ticket
        public Ticket Ticket { get; set; }

        public int TotalTickets { get; set; }= 0;

        // Enums
        public IEnumerable<SelectListItem>? StatusOptions { get; set; }
        public IEnumerable<SelectListItem> TypeOfIncidentOptions =>
             Enum.GetValues<TypeOfIncident>()
            .Cast<TypeOfIncident>()
            .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });
        public IEnumerable<SelectListItem>? PriorityOptions { get; set; }

        public List<Ticket> TicketList { get; set; } = new List<Ticket>();

        public List<Employee> PotentialTransferees { get; set; } = new List<Employee>();
    }
}
