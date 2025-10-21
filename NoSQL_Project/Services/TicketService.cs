using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepo;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepo = ticketRepository;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _ticketRepo.GetAllTicketsAsync();
        }

        public async Task<List<Ticket>> GetTicketsByEmployeeIdAsync(EmployeeDetails employee)
        {
            var result = await _ticketRepo.GetTicketsByEmployeeIdAsync(employee);
            return result ?? new List<Ticket>();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            await _ticketRepo.UpdateTicketAsync(ticket);
        }
        public async Task CreateTicketAsync(Ticket ticket)
        {
            await _ticketRepo.CreateTicketAsync(ticket);
        }

        public async Task<Ticket> GetByIdAsync(string id)
        {
            return await _ticketRepo.GetByIdAsync(id);
        }

        public TicketViewModel FillTicketInfo(Ticket ticket)
        {
            return _ticketRepo.FillTicketInfo(ticket);
        }

        public Task<bool> CloseAsync(Ticket ticket)
        {
            return _ticketRepo.CloseAsync(ticket);
        }

       
    }
}
