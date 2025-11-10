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

        public Task<(int total, int resolved, int transferred)> GetEmployeeStatsAsync(string firstName, string lastName)
        {
            return  _ticketRepo.GetEmployeeStatsAsync(firstName, lastName);
        }

        public Task<List<Ticket>> SearchTicketsAsync(string searchText, bool useAnd)
        {
            return _ticketRepo.SearchTicketsAsync(searchText, useAnd);
        }

        public Task AddResolutionStep(string ticketId, EmployeeDetails details)
        {
            return _ticketRepo.AddResolutionStep(ticketId, details);
        }

        public Task AssignMyselfToTicket(Ticket ticket, EmployeeDetails details)
        {
            if (ticket.ResolutionSteps == null || !ticket.ResolutionSteps.Any())
            {
                return _ticketRepo.AddResolutionStep(ticket.TicketId, details);
            }
            throw new InvalidOperationException("This ticket is assigned to someone else/you're already assigned to it.");
        }

        public EscalateViewModel FillEscalateInfo(Ticket ticket)
        {
            return _ticketRepo.FillEscalateInfo(ticket);
        }
    }
}
