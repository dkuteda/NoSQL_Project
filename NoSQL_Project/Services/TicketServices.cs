using NoSQL_Project.Repositories;
using NoSQL_Project.Repositories.Interfaces;

namespace NoSQL_Project.Services
{
    public class TicketServices
    {
        private readonly ITicketRepository _ticketRepo;

        public TicketServices(ITicketRepository ticketRepository)
        {
            _ticketRepo = ticketRepository;
        }
    }
}
