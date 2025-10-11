using NoSQL_Project.Models;

namespace NoSQL_Project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<List<Ticket>> GellAsync();
    }
}
