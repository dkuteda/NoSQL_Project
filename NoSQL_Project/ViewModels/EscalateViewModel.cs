using NoSQL_Project.Enums;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class EscalateViewModel
    {
        public Ticket Ticket { get; set; }

        public Priority NewPriority { get; set; }

        public DateTime NewDeadline { get; set; }

        public EscalateViewModel(Ticket ticket, Priority newPriority, DateTime newDeadline)
        {
            Ticket = ticket;
            NewPriority = newPriority;
            NewDeadline = newDeadline;
        }
    }
}
