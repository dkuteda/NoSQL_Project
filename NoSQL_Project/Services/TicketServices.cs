using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services.Interfaces;

namespace NoSQL_Project.Services
{
    public class TicketServices : ITicketServices
    {
        private readonly ITicketRepository _ticketRepo;

        public TicketServices(ITicketRepository ticketRepository)
        {
            _ticketRepo = ticketRepository;
        }

        public async Task<List<Ticket>> GellAsync()
        {
            return await _ticketRepo.GellAsync();
        }
    }
}
