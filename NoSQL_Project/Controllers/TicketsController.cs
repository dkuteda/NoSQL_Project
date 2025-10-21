using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    [Route("MyTickets")]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService) => _ticketService = ticketService;

        public async Task<IActionResult> Index()
        {

            List<Ticket> tickets = await _ticketService.GetAllTicketsAsync();
            var model = new TicketViewModel()
            {
                TicketList = tickets
            };

            return View("MyTickets", model);
        }

        [HttpGet("TicketDetails")]
        public IActionResult TicketDetails(string id)
        {
            
            var ticket = _ticketService.GetByIdAsync(id).Result;
            if (ticket == null) return NotFound();
            var employeeId = HttpContext.Session.GetString("EmployeeId") ?? string.Empty;
            bool isAssignee = ticket.HandledBy != null && ticket.HandledBy.EmployeeId == employeeId;
            ViewData["isAssignee"] = isAssignee;
            return View(ticket);
        }

        [HttpGet("TicketDashboard")]
        public async Task<IActionResult> TicketDashboard()
        {
            List<Ticket> tickets = await _ticketService.GetAllTicketsAsync();
            var model = new TicketViewModel()
            {
                TicketList = tickets
            };
            return View();
        }

        [HttpGet ("UpdateTicket")]
        public IActionResult UpdateTicket(string id)
        {
            var ticket = _ticketService.GetByIdAsync(id).Result; // Synchronously wait for the result
            if (ticket == null)
            {
                return NotFound();
            }
            var ViewModel = _ticketService.FillTicketInfo(ticket);

            return View(ViewModel);
        }

        [HttpPost ("UpdateTicket")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicket(TicketViewModel ticketViewModel)
        {
            try
            {
                await _ticketService.UpdateTicketAsync(ticketViewModel.Ticket);
                TempData["SuccessMessage"] = "Ticket has been updated successfully";
                return Redirect("/MyTickets");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                return View(ticketViewModel);
            }
        }

        [HttpGet("AddTicket")]
        public IActionResult AddTicket()
        {
            var employeeId = HttpContext.Session.GetString("EmployeeId") ?? string.Empty;
            var employeeName = HttpContext.Session.GetString("EmployeeName") ?? string.Empty;
            ViewData["EmployeeDetails"] = new EmployeeDetails()
            {
                EmployeeId = employeeId,
                FirstName = employeeName
            };
            var viewModel = new TicketViewModel
            {
                Ticket = new Ticket
                {
                    TicketId = Guid.NewGuid().ToString() 
                },
                TypeOfIncidentOptions = Enum.GetValues(typeof(TypeOfIncident))
                    .Cast<TypeOfIncident>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = e.ToString()
                    })
            };
            return View(viewModel);
        }

        [HttpPost("AddTicket")]
        public async Task<IActionResult> AddTicket(TicketViewModel model)
        {
            try
            {
                await _ticketService.CreateTicketAsync(model.Ticket);

                TempData["ConfirmMessage"] = "Ticket has been created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = $"{ex}";
                return View("AddTicket", model);
            }
        }

        [HttpGet("CloseTicket")]
        public IActionResult CloseTicket(string id)
        {
            var ticket = _ticketService.GetByIdAsync(id).Result; // Synchronously wait for the result
            if (ticket == null)
            {
                return NotFound();
            }
            var ViewModel = _ticketService.FillTicketInfo(ticket);

            return View(ViewModel);
        }

        [HttpPost("CloseTicket")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(Ticket ticket)
        {
            try
            {
                bool isClosed = await _ticketService.CloseAsync(ticket);
                if (isClosed)
                {
                    TempData["SuccessMessage"] = "Ticket has been closed successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ticket not found or already closed";
                }
                return Redirect("/MyTickets");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                var viewModel = new TicketViewModel
                {
                    Ticket = ticket,
                };
                return View(viewModel);
            }
        }
    }
}
