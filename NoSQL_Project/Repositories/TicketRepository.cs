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
        private readonly IMongoCollection<Employee> _employees;

        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("tickets");
            _employees = db.GetCollection<Employee>("Employees");
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _tickets
                .Find(s => true)
                .SortByDescending(e => e.Status)  // open first
                .ThenBy(e => e.Priority)          // higher priority first
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
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                Priority = ticket.Priority,
                Deadline = ticket.Deadline,
                CreatedAt = ticket.CreatedAt,
                HandledBy = ticket.HandledBy,
                TypeOfIncident = ticket.TypeOfIncident,
                ResolutionSteps = ticket.ResolutionSteps.Select(resolutionStep => new ResolutionStepViewModel
                {
                    ResolutionStepNr = resolutionStep.ResolutionStepNr,
                    Action = resolutionStep.Action,
                    PresentHandlerName = resolutionStep.PresentHandler // You can populate this if you have access to the Employee collection
                }).ToList(),

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

        public Ticket ViewModelToTicket(TicketViewModel ticketViewModel)
        {
            foreach (Ticket ticket in ticketViewModel.TicketList)
            {
                if (ticket.TicketId == ticketViewModel.TicketId)
                {
                    ticket.Title = ticketViewModel.Title;
                    ticket.Description = ticketViewModel.Description;
                    ticket.Status = ticketViewModel.Status;
                    ticket.Priority = ticketViewModel.Priority;
                    ticket.Deadline = ticketViewModel.Deadline;
                    ticket.CreatedBy = ticketViewModel.CreatedBy.FirstName + ticketViewModel.CreatedBy.LastName;
                    ticket.HandledBy = ticketViewModel.HandledBy;
                    ticket.CreatedAt = ticketViewModel.CreatedAt;
                    ticket.TypeOfIncident = ticketViewModel.TypeOfIncident;
                    ticket.ResolutionSteps = ticketViewModel.ResolutionSteps.Select(viewModel => new ResolutionStep
                    {
                        ResolutionStepNr = viewModel.ResolutionStepNr,
                        Action = viewModel.Action,
                        PresentHandler = viewModel.PresentHandlerName
                    }).ToList();
                    return ticket;
                }
            }

            return null;
        }
    }
}
