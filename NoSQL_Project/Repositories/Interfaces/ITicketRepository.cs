using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<List<TicketViewModel>> GetAllTicketsAsync();

        Task UpdateTicketAsync(Ticket ticket);
    }
}
