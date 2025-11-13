using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class TicketViewModel
    {
        //used for CRUD of a single ticket
        public Ticket? Ticket { get; set; }
        public List<Ticket> TicketList { get; set; } = new List<Ticket>();
        public List<Employee> PotentialTransferees { get; set; } = new List<Employee>();

        // Enums
        public IEnumerable<SelectListItem>? StatusOptions { get; set; }
               = Enum.GetValues(typeof(TicketStatus))
                     .Cast<TicketStatus>()
                     .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() });
        public IEnumerable<SelectListItem> TypeOfIncidentOptions =>
                 Enum.GetValues<TypeOfIncident>().Cast<TypeOfIncident>()
                     .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });
        public IEnumerable<SelectListItem>? PriorityOptions { get; set; } =
                 Enum.GetValues(typeof(Priority))
                     .Cast<Priority>()
                     .Select(p => new SelectListItem
                     {
                         Text = p.ToString(),
                         Value = p.ToString()
                     });

        public TicketViewModel(Ticket ticket, List<Ticket> tickets, List<Employee> potentialTransferees)
        {
            Ticket = ticket;
            TicketList = tickets;
            PotentialTransferees = potentialTransferees;
        }

        public TicketViewModel(List<Ticket> tickets)
        {
            TicketList = tickets;
        }
        public TicketViewModel(Ticket ticket)
        {
            Ticket = ticket;
        }
        public TicketViewModel(Ticket ticket, List<Employee> potentialTransferees)
        {
            Ticket = ticket;
            PotentialTransferees = potentialTransferees;
        }
        public TicketViewModel(List<Employee>potentialTransferees)
        {
            PotentialTransferees = potentialTransferees;
        }

        public TicketViewModel () { }
    }
}
