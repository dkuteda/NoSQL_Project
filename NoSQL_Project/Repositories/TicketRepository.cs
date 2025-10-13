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

        public async Task<List<Ticket>> GetAllTicketsFromDBAsync()
        {
            return await _tickets
                .Find(s => true)
                .SortByDescending(e => e.Status)  // open first
                .ThenBy(e => e.Priority)          // higher priority first
                .ToListAsync();
        }

        // Helper to get all unique and only the employee IDs needed for tickets
        private static List<string> GetAllEmployeeIds(List<Ticket> tickets)
        {
            var allHandlerIds = tickets
                .SelectMany(t => t.ResolutionSteps)
                .Select(r => r.PresentHandler)
                .Where(id => !string.IsNullOrEmpty(id));

            var createdByIds = tickets.Select(t => t.CreatedBy).Where(id => !string.IsNullOrEmpty(id));
            var handledByIds = tickets.Select(t => t.HandledBy).Where(id => !string.IsNullOrEmpty(id));

            return allHandlerIds
                .Concat(createdByIds)
                .Concat(handledByIds)
                .Distinct()
                .ToList();
        }

        // Helper to build employee dictionary
        private static Dictionary<string, Employee> BuildEmployeeDictionary(List<Employee> employees)
        {
            return employees.ToDictionary(e => e.EmployeeId, e => e);
        }

        // Helper to map ResolutionSteps to ViewModels
        private static List<ResolutionStepViewModel> MapResolutionSteps(
            List<ResolutionStep> steps, Dictionary<string, Employee> employeeDict)
        {
            return steps.Select(r =>
            {
                employeeDict.TryGetValue(r.PresentHandler, out var handler);
                return new ResolutionStepViewModel
                {
                    ResolutionStepNr = r.ResolutionStepNr,
                    Action = r.Action,
                    PresentHandlerName = handler != null
                        ? $"{handler.FirstName} {handler.LastName}"
                        : "Unknown"
                };
            }).ToList();
        }

        // Helper to map a Ticket to TicketViewModel
        private static TicketViewModel MapTicketToViewModel(
            Ticket ticket, Dictionary<string, Employee> employeeDict)
        {
            var resolutionSteps = MapResolutionSteps(ticket.ResolutionSteps, employeeDict);

            employeeDict.TryGetValue(ticket.CreatedBy, out var createdByEmployee);
            employeeDict.TryGetValue(ticket.HandledBy, out var handledByEmployee);
            var handledByName = handledByEmployee != null ? $"{handledByEmployee.FirstName} {handledByEmployee.LastName}" : "Unknown";

            var viewModel = new TicketViewModel
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                Priority = ticket.Priority,
                Deadline = ticket.Deadline,
                CreatedAt = ticket.CreatedAt,
                CreatedBy = createdByEmployee,
                HandledBy = handledByName,
                TypeOfIncident = ticket.TypeOfIncident,
                ResolutionSteps = resolutionSteps
            };

            return viewModel;
        }

        public async Task<List<TicketViewModel>> GetAllTicketsAsync()
        {
            // 1. Get all tickets
            var tickets = await GetAllTicketsFromDBAsync();

            // 2. Get all unique employee IDs
            var allEmployeeIds = GetAllEmployeeIds(tickets);

            // 3. Load all employees in one query
            var employees = await _employees
                .Find(e => allEmployeeIds.Contains(e.EmployeeId))
                .ToListAsync();

            // 4. Build a dictionary for quick lookup
            var employeeDict = BuildEmployeeDictionary(employees);

            // 5. Map tickets to TicketViewModel
            var ticketViewModels = tickets
                .Select(ticket => MapTicketToViewModel(ticket, employeeDict))
                .ToList();

            return ticketViewModels;
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
