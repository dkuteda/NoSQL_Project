using Microsoft.AspNetCore.Mvc;
using NoSQL_Project.Enums;
using NoSQL_Project.Models;
using NoSQL_Project.Services;
using NoSQL_Project.Services.Interfaces;
using NoSQL_Project.ViewModels;

namespace NoSQL_Project.Controllers
{
    [Route("[controller]/[action]")]
    public class TicketsController : BaseController
    {
        private readonly ITicketService _ticketService;
        private readonly IEmployeeService _employeeService;

        public TicketsController(ITicketService ticketService, IEmployeeService employeeService)
        {
            _ticketService = ticketService;
            _employeeService = employeeService;
        }

        //Nana Yaa's Code
        public async Task<IActionResult> Index()
        {
            Employee? employee = this.CurrentUser;

            List<Ticket> tickets = await _ticketService.GetTicketsByEmployeeIdAsync(employee);
            TicketViewModel model = new TicketViewModel(tickets);
            return View("MyTickets", model);
        }

        [HttpGet("TicketDetails")]
        public async Task<IActionResult> TicketDetails(string id)
        {
            EmployeeDetails presentHandler;
            bool isAssignee = false;
            Ticket ticket = await _ticketService.GetByIdAsync(id);
            TicketViewModel model = new TicketViewModel(ticket);
            //TO DO: do something in case ticket is null
            if (ticket.ResolutionSteps.Count != 0)
            {
                presentHandler = ticket.ResolutionSteps.Last().PresentHandler;
                isAssignee = presentHandler != null && presentHandler.EmployeeId == this.CurrentUser.EmployeeId;
            }
            ViewData["LoggedInEmployee"] = this.CurrentUser;
            ViewData["IsAssignee"] = isAssignee;
            return View(model);
        }

        [HttpPost]
        public IActionResult AssignMyselfToTicket(Ticket ticket, EmployeeDetails details)
        {
            try
            {
                _ticketService.AssignMyselfToTicket(ticket, details);
                return RedirectToAction("TicketDetails", new { id = ticket.TicketId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("TicketDetails", new { id = ticket.TicketId });
            }
        }
        [HttpGet] //TO DO: Figure out a try catch here
        public async Task<IActionResult> OnSearch(string nameQuery, string ticketId)
        {
            List<Employee> potentialTransferees = await _employeeService.AutocompleteSearchEmployees(nameQuery);
            TicketViewModel model = new TicketViewModel(await _ticketService.GetByIdAsync(ticketId), potentialTransferees);
            ViewData["LoggedInEmployee"] = this.CurrentUser;
            return View("TicketDetails", model);
        }

        [HttpPost]
        public IActionResult TransferTicket(string ticketId, EmployeeDetails details)
        {
            try
            {
                _ticketService.AddResolutionStep(ticketId, details);
                return RedirectToAction("TicketDetails", new { id = ticketId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("TicketDetails", new { id = ticketId });
            }
        }

        [HttpGet("AddTicket")]
        public IActionResult AddTicket()
        {
            ViewData["LoggedInEmployee"] = this.CurrentUser;
            Ticket ticket = new() { TicketId = Guid.NewGuid().ToString() };
            var viewModel = new TicketViewModel(ticket);
            return View(viewModel);
        }

        [HttpPost("AddTicket")]
        public async Task<IActionResult> AddTicket(TicketViewModel model)
        {
            try
            {
                model.Ticket.ResolutionSteps = model.Ticket.ResolutionSteps ?? new List<ResolutionStep>();
                await _ticketService.CreateTicketAsync(model.Ticket);

                TempData["SuccessMessage"] = "Ticket has been created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ErrorMessage = $"{ex}";
                return View("AddTicket", model);
            }
        }

        //Thijmen's Code
        [HttpGet("UpdateTicket")]
        public IActionResult UpdateTicket(string id)
        {
            var ticket = _ticketService.GetByIdAsync(id).Result; // Synchronously wait for the result
            if (ticket == null)
            {
                return NotFound();
            }
            var ViewModel = new TicketViewModel(ticket);

            return View(ViewModel);
        }

        [HttpPost("UpdateTicket")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicket(TicketViewModel ticketViewModel)
        {
            try
            {
                await _ticketService.UpdateTicketAsync(ticketViewModel.Ticket);
                TempData["SuccessMessage"] = "Ticket has been updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                return View(ticketViewModel);
            }
        }

        [HttpGet("CloseTicket")]
        public IActionResult CloseTicket(string id)
        {
            var ticket = _ticketService.GetByIdAsync(id).Result;
            if (ticket == null)
            {
                return NotFound();
            }
            var ViewModel = new TicketViewModel(ticket);

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
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                var viewModel = new TicketViewModel(ticket);
                return View("Dashboard", viewModel);
            }
        }

        [HttpGet]
        public IActionResult EscalateTicket(string id)
        {
            var ticket = _ticketService.GetByIdAsync(id).Result;
            if (ticket == null)
            {
                return NotFound();
            }
            var ViewModel = _ticketService.FillEscalateInfo(ticket);

            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EscalateTicket(EscalateViewModel escalationTicket)
        {
            try
            {
                await _ticketService.UpdateEscalation(escalationTicket);
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exception occurred: {ex.Message}";
                return View("Dashboard");
            }
        }

        //Hamza's Code
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard(string text, bool and = false)
        {
            var employee = HttpContext.Session.GetObject<Employee>("LoggedInUser");

            List<Ticket> tickets;
            bool CanCloseTicket = false;

            if (!string.IsNullOrWhiteSpace(text))
            {
                // ✅ search mode
                tickets = await _ticketService.SearchTicketsAsync(text, and);
            }
            else
            {
                // ✅ default dashboard load
                if (employee.UserRole == UserRole.employee)
                    tickets = await _ticketService.GetTicketsByEmployeeIdAsync(employee);
                else
                {
                    tickets = await _ticketService.GetAllTicketsAsync();
                    CanCloseTicket = true;
                }
            }

            int total = tickets.Count;
            int open = tickets.Count(t => t.Status == TicketStatus.open);
            int resolved = tickets.Count(t => t.Status == TicketStatus.resolved);
            int closed = tickets.Count(t => t.Status == TicketStatus.closed);

            var model = new DashboardViewModel
            {
                TotalTickets = total,
                OpenPercent = total > 0 ? (open * 100) / total : 0,
                ResolvedPercent = total > 0 ? (resolved * 100) / total : 0,
                ClosedPercent = total > 0 ? (closed * 100) / total : 0,
                TicketList = tickets.Take(5).ToList(), // normal dashboard limit
                MayCloseTicket = CanCloseTicket
            };

            ViewBag.SearchText = text; // optional (keep input value)
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string text, bool and = false)
        {
            var results = await _ticketService.SearchTicketsAsync(text, and);
            var model = new TicketViewModel(results);
            return View("MyTickets", model);
        }
    }

}

