using NoSQL_Project.Models;

namespace NoSQL_Project.Services.Interfaces
{
    public interface ITicketServices
    {
        Task<List<Ticket>> GellAsync();
    }
}
