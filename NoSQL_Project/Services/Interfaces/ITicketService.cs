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
        Task<List<Ticket>> GetTicketsByEmployeeIdAsync(EmployeeDetails employee);

        Task CreateTicketAsync(Ticket ticket);

        Task<bool> CloseAsync(Ticket ticket);

        Task<(int total, int resolved, int transferred)> GetEmployeeStatsAsync(string firstName, string lastName);

        Task<List<Ticket>> SearchTicketsAsync(string searchText, bool useAnd);

<<<<<<< HEAD
        EscalateViewModel FillEscalateInfo(Ticket ticket);
=======
        Task AddResolutionStep(string ticketId, EmployeeDetails details);
        Task AssignMyselfToTicket(Ticket ticket, EmployeeDetails details);
>>>>>>> 97600f9e0183fb39a435bd6339b12c09ca18e65a

    }
}
