using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Services;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    [Route("TicketDashboard")]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService) => _ticketService = ticketService;

        public async Task<IActionResult> Index()
        {
            List<Ticket> tickets = await _ticketService.GetAllTicketsAsync();
            TicketViewModel ticketViewModel = new TicketViewModel()
            {
                TicketList = tickets
            };

            return View("TicketDashboard", ticketViewModel);
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
                return Redirect("/TicketDashboard");
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
            //  Employee? loggedInEmployee = HttpContext.Session.GetObject<Employee>("LoggedInEmployee");
            var viewModel = new TicketViewModel
            {
                Ticket = new Ticket
                {
                    TicketId = Guid.NewGuid().ToString() // Set required TicketId
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

        [HttpPost]
        public async Task<IActionResult> AddTicket(Ticket ticket)
        {
            try
            {
                await _ticketService.CreateTicketAsync(ticket);

                TempData["ConfirmMessage"] = "User has been created correctly";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = $"{ex}";
                return View(ticket);
            }
        }
    }
}
