using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class TicketViewModel
    {
        public Ticket? Ticket { get; set; } //used for CRUD of a single ticket
        public List<Ticket> TicketList { get; set; } = new List<Ticket>();
        public List<Employee> PotentialTransferees { get; set; } = new List<Employee>();

        //Calculated/computed properties
        public EmployeeDetails? LastPresentHandler => Ticket?.ResolutionSteps?.LastOrDefault()?.PresentHandler;
        public bool IsAssignee { get; private set; } = false;
        public bool CanAssignToSelf { get; private set; } = false;
        int totalTickets => TicketList.Count;
        int openTickets => TicketList.Count(t => t.Status == TicketStatus.open);
        int resolvedTickets => TicketList.Count(t => t.Status == TicketStatus.resolved);
        int closedTickets => TicketList.Count(t => t.Status == TicketStatus.closed);

        public double percentOpenTickets =>
            totalTickets == 0 ? 0 : (double)openTickets / totalTickets * 100;
        public double percentResolvedTickets =>
            totalTickets == 0 ? 0 : (double)resolvedTickets / totalTickets * 100;
        public double percentClosedTickets =>
            totalTickets == 0 ? 0 : (double)closedTickets / totalTickets * 100;

        // Properties to hold SelectListItems for dropdowns
        public IEnumerable<SelectListItem>? StatusOptions { get; set; }
               = Enum.GetValues(typeof(TicketStatus))
                     .Cast<TicketStatus>()
                     .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() });
        public IEnumerable<SelectListItem> TypeOfIncidentOptions =>
                 Enum.GetValues<TypeOfIncident>().Cast<TypeOfIncident>()
                     .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });

        //Constructors
        public TicketViewModel() { }
        public TicketViewModel(List<Ticket> tickets)
        {
            TicketList = tickets;
        }
        public TicketViewModel(Ticket ticket)
        {
            Ticket = ticket;
        }
        public TicketViewModel(Ticket ticket, Employee employee)
        {
            Ticket = ticket;
            IsAssignee = LastPresentHandler?.EmployeeId == employee.EmployeeId
             && employee.UserRole != UserRole.employee;
            CanAssignToSelf = !IsAssignee
                              && employee.UserRole != UserRole.employee;
        }
        public TicketViewModel(Ticket ticket, Employee employee, List<Employee> potentialTransferees)
        {
            Ticket = ticket;
            PotentialTransferees = potentialTransferees;
            IsAssignee = LastPresentHandler?.EmployeeId == employee.EmployeeId 
                         && employee.UserRole != UserRole.employee;
            CanAssignToSelf = !IsAssignee
                              && employee.UserRole != UserRole.employee;
        }
    }
}
