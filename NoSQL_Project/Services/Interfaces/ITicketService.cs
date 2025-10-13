using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services.Interfaces
{
    public interface ITicketService
    {
        Task<List<TicketViewModel>> GetAllTicketsAsync();

        Task UpdateTicketAsync(Ticket ticket);
    }
}
