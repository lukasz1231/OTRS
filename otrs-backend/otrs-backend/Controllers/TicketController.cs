using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using otrs_backend.Requests;
using otrs_backend.Services;
using System.Security.Claims;

namespace otrs_backend.Controllers
{
    [Route("api/ticket")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public TicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }

            try
            {
                var ticket = await _ticketService.CreateTicketAsync(request, currentUserId);
                return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }
            return Ok(new { id = id, message = "Endpoint w budowie" });
        }

        // NOWY ENDPOINT: Obsługa zapytania GET /api/ticket
        [HttpGet]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }

            try
            {
                // Odpytujemy nasz nowy, czysty serwis
                var tickets = await _ticketService.GetMyTicketsAsync(currentUserId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania zgłoszeń", error = ex.Message });
            }
        }
    }
}