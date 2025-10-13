using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services.Interfaces
{
    public interface ITicketService
    {
        Task UpdateTicketAsync(Ticket ticket);

        Task<Ticket> GetByIdAsync(string id);

        TicketViewModel FillTicketInfo(Ticket ticket);

        Task<List<Ticket>> GetAllTicketsAsync();
        Task CreateTicketAsync(Ticket ticket);
    }
}
