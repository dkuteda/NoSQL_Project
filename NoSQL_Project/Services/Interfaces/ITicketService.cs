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
<<<<<<< HEAD

        Task<bool> CloseAsync(Ticket ticket);
=======
        Task CreateTicketAsync(Ticket ticket);
>>>>>>> 5e5bbe753a397afaf48e656389efeea2971bd478
    }
}
