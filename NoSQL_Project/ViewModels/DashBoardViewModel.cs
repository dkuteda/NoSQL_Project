using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Models;

namespace NoSQL_Project.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenPercent { get; set; }
        public int ResolvedPercent { get; set; }
        public int ClosedPercent { get; set; }

        public List<Ticket> TicketList { get; set; } = new List<Ticket>();
    }
}