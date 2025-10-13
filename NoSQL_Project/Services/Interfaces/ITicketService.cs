using NoSQL_Project.Models;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services.Interfaces
{
    public interface ITicketService
    {
        Task UpdateTicketAsync(Ticket ticket);

        Task<Ticket> GetByIdAsync(string id);

        TicketViewModel FillTicketInfo(Ticket ticket);

        Ticket ViewModelToTicket(TicketViewModel ticketViewModel);

        Task<List<Ticket>> GetAllTicketsAsync();
    }
}
