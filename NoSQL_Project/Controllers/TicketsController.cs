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
            Ticket ticket = _ticketService.ViewModelToTicket(ticketViewModel);
            try
            {
                if (ticket == null)
                {
                    throw new Exception("TicketId not found");
                }
                await _ticketService.UpdateTicketAsync(ticket);
                TempData["SuccessMessage"] = "Ticket has been updated successfully";
                return RedirectToAction("TicketDashboard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                return View(ticketViewModel);
            }
        }
    }
}
