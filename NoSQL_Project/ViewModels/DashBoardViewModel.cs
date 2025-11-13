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

     

<<<<<<< HEAD
        public int ResolvedTickets { get; set; }
        public int TransferredTickets { get; set; }
        public bool ShowReport { get; set; } = false;

        public bool MayCloseTicket { get; set; }
=======
       
>>>>>>> abd96b1229be7f9e610bdb44061fc58e1bc2a4ec
    }
}