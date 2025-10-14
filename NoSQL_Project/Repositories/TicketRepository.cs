using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Repositories.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("tickets");
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _tickets
                .Find(s => true)
                .SortByDescending(e => e.Status)  // open first
                .ThenBy(e => e.Priority)          // higher priority first
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetTicketsByEmployeeIdAsync(EmployeeDetails employee)
        {
            return await _tickets
                .Find(t => t.CreatedBy.EmployeeId == employee.EmployeeId)
                .SortByDescending(e => e.Status)
                .ThenBy(e => e.Priority)
                .ToListAsync();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            await _tickets.ReplaceOneAsync(s => s.TicketId == ticket.TicketId, ticket);
        }

        public async Task<Ticket> GetByIdAsync(string id)
        {
            return await _tickets.Find(s => s.TicketId == id).FirstOrDefaultAsync();
        }

        public TicketViewModel FillTicketInfo(Ticket ticket)
        {
            return new TicketViewModel
            {
                Ticket = ticket,
                // Enums converted to select options
                StatusOptions = Enum.GetValues(typeof(TicketStatus))
            .Cast<TicketStatus>()
            .Select(s => new SelectListItem { Text = s.ToString(), Value = s.ToString() }),

                TypeOfIncidentOptions = Enum.GetValues(typeof(TypeOfIncident))
            .Cast<TypeOfIncident>()
            .Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() }),

                PriorityOptions = Enum.GetValues(typeof(Priority))
            .Cast<Priority>()
            .Select(p => new SelectListItem { Text = p.ToString(), Value = p.ToString() })
            };
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            await _tickets.InsertOneAsync(ticket);
        }

        public async Task<bool> CloseAsync(Ticket ticket)
        {
            var filter = Builders<Ticket>.Filter.Eq(e => e.TicketId, ticket.TicketId);
            var update = Builders<Ticket>.Update.Set(e => e.Status, TicketStatus.closed);

            var result = await _tickets.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
